using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Models;
using BLL;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using XC.RSAUtil;
using JWT.Algorithms;
using JWT;
using JWT.Serializers;
using JWT.Exceptions;
using System.Web.Script.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using StackExchange.Redis;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MvcApp.Controllers
{
    public class BaseController : Controller
    {

        #region token类
        protected class TokenInfo
        {
            public string UserName { get; set; }
            public string UserId { get; set; }
            public string Userphoto { get; set; }
            public string sign { get; set; }
            public DateTime time { get; set; }
        }

        protected class TokenHelper
        {
            public static string SecretKey { get; set; }//这个服务端加密秘钥 属于私钥
            private static JavaScriptSerializer myJson = new JavaScriptSerializer();

            public static string GenToken(TokenInfo token)
            {
                var load = new Dictionary<string, dynamic>
            {
                {"UserId",token.UserId },
                { "UserName",token.UserName },
                { "Userphoto",token.Userphoto },
                { "sign",token.sign },
                {"time",token.time }
            };
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                return encoder.Encode(load, SecretKey);
            }
        }
        #endregion

        #region 生成token
        protected string gettoken(string UserId, string UserName, string Userphoto, DateTime time)
        {
            TokenInfo tokenInfo = new TokenInfo();
            //向token添加信息
            tokenInfo.UserId = UserId;
            tokenInfo.UserName = UserName;
            tokenInfo.Userphoto = Userphoto;
            tokenInfo.time = time;
            //关键字段
            string content = UserId + UserName + Userphoto + time.ToString();
            //对关键字段生成签名
            string priKey;
            if (Session["Private"] == null)
            {
                StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(@"\priKey.txt"), System.Text.Encoding.Default);
                priKey = sr.ReadToEnd();
                sr.Close();
                Session["Private"] = priKey;
            }
            else
            {
                priKey = Session["Private"].ToString();
            }
            string sign = Sign(content, priKey);
            tokenInfo.sign = sign;
            TokenHelper.SecretKey = priKey;
            string valueToken = TokenHelper.GenToken(tokenInfo);
            return valueToken;
        }
        #endregion

        #region 读取token
        protected JObject readtoken(string tokenContent)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtAlgorithm alg = new HMACSHA256Algorithm();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, alg);
            try
            {
                string priKey;
                if (Session["Private"] == null)
                {
                    StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(@"\priKey.txt"), System.Text.Encoding.Default);
                    priKey = sr.ReadToEnd();
                    sr.Close();
                    Session["Private"] = priKey;
                }
                else
                {
                    priKey = Session["Private"].ToString();
                }
                var json = decoder.Decode(tokenContent, priKey, true);
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                //校验通过，返回解密后的json对象
                return jo;
            }
            catch (Exception)
            {
                Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                RedirectToAction("Index", "Animation");
                return null;
            }
        }
        #endregion

        #region 验证token
        protected bool VerToken(string tokenContent, string pubKey)
        {
            try
            {
                //读取token内容，返回json对象
                JObject list = readtoken(tokenContent);
                //获取签名
                string sign = list["sign"].ToString();
                string name = list["UserName"].ToString();
                string redisName = Encryption(name, "");
                if (RedisHelper.Exists(redisName))    //检查token是否存在于令牌池，不存在则说明该token已被注销
                {
                    //检查是否有同一用户重复令牌
                    if (RedisHelper.Get(redisName).ToString() != sign)
                    {
                        return false;
                    }
                    //比较token有效时间
                    DateTime dateTime = DateTime.Now;
                    DateTime Expiration = (DateTime)list["time"];
                    Expiration=Expiration.AddDays(1);   //令牌有效时间
                    TimeSpan timeSpan = Expiration - dateTime;  //计算时间间隔
                    double mineutes = timeSpan.TotalMinutes;
                    //若间隔小于零则已经过期
                    if (mineutes > 0)
                    {
                        //token临近有效期，重新签发token
                        if (mineutes < 30)
                        {
                            //移除当前token
                            Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                            //从令牌池中移除对应token
                            RedisHelper.Remove(redisName);
                            // 更新token并重新验证
                            string newToken = gettoken(list["UserId"].ToString(), list["UserName"].ToString(), list["Userphoto"].ToString(), DateTime.Now);
                            if (VerToken(newToken, pubKey))
                            {
                                //验证通过后将新token传递给客户端
                                HttpCookie cookie = new HttpCookie("Login");
                                cookie.Values.Add("Token", newToken);
                                cookie.Expires = DateTime.Now.AddHours(1);
                                Response.Cookies.Add(cookie);
                                //将新token放入令牌池，作为有效令牌，登录时设置有效期为1小时，键名为用户名，值为签名
                                string newsign = readtoken(newToken)["sign"].ToString();
                                RedisHelper.Set(redisName, newsign, true, 1);
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    else
                    {
                        //token过期，验证失败
                        return false;
                    }
                    //拼接字符串生成关键字段
                    string content = list["UserId"].ToString() + list["UserName"].ToString() + list["Userphoto"].ToString() + list["time"].ToString();
                    //验证token内容是否被篡改
                    bool result = verify(content, sign, pubKey);
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 签名
        protected static string Sign(string content, string privateKey)
        {
            privateKey = RsaKeyConvert.PrivateKeyPkcs8ToXml(privateKey);
            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(content);
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] rgbHash = sha256.ComputeHash(bt);

            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(privateKey);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("SHA256");
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);

        }
        #endregion

        #region 验证签名
        protected static bool verify(string content, string signedString, string publicKey)
        {
            signedString = signedString.Replace(" ", "+");
            publicKey = publicKey.Replace("%0d", "\r").Replace("%0a", "\n");
            publicKey = RsaKeyConvert.PublicKeyPemToXml(publicKey);
            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(content);
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] rgbHash = sha256.ComputeHash(bt);

            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(publicKey);
            RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
            deformatter.SetHashAlgorithm("SHA256");
            byte[] rgbSignature = Convert.FromBase64String(signedString);
            if (deformatter.VerifySignature(rgbHash, rgbSignature))
            {
                return true;
            }
            return false;

        }
        #endregion

        #region Redis管理类
        public static class RedisHelper
        {
            private static string Constr = "127.0.0.1:6379,password=123456,DefaultDatabase=0";

            private static object _locker = new Object();
            private static ConnectionMultiplexer _instance = null;

            /// <summary>
            /// 使用一个静态属性来返回已连接的实例，如下列中所示。这样，一旦 ConnectionMultiplexer 断开连接，便可以初始化新的连接实例。
            /// </summary>
            public static ConnectionMultiplexer Instance
            {
                get
                {
                    if (Constr.Length == 0)
                    {
                        throw new Exception("连接字符串未设置！");
                    }
                    if (_instance == null)
                    {
                        lock (_locker)
                        {
                            if (_instance == null || !_instance.IsConnected)
                            {
                                _instance = ConnectionMultiplexer.Connect(Constr);
                            }
                        }
                    }
                    //注册如下事件
                    _instance.ConnectionFailed += MuxerConnectionFailed;
                    _instance.ConnectionRestored += MuxerConnectionRestored;
                    _instance.ErrorMessage += MuxerErrorMessage;
                    _instance.ConfigurationChanged += MuxerConfigurationChanged;
                    _instance.HashSlotMoved += MuxerHashSlotMoved;
                    _instance.InternalError += MuxerInternalError;
                    return _instance;
                }
            }

            static RedisHelper()
            {
            }

            public static void SetCon(string config)
            {
                Constr = config;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static IDatabase GetDatabase()
            {
                return Instance.GetDatabase();
            }

            /// <summary>
            /// 这里的 MergeKey 用来拼接 Key 的前缀，具体不同的业务模块使用不同的前缀。
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            private static string MergeKey(string key)
            {
                return key;
                //return BaseSystemInfo.SystemCode + key;
            }

            /// <summary>
            /// 根据key获取缓存对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T Get<T>(string key)
            {
                key = MergeKey(key);
                return Deserialize<T>(GetDatabase().StringGet(key));
            }

            /// <summary>
            /// 根据key获取缓存对象
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static object Get(string key)
            {
                key = MergeKey(key);
                return Deserialize<object>(GetDatabase().StringGet(key));
            }

            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="isHour"></param>
            /// <param name="expire"></param>
            public static void Set(string key, object value, bool isHour, int expire = 0)
            {
                key = MergeKey(key);
                if (expire > 0)
                {
                    if (isHour) //判断是否为延长时间，如果是延长时间则有效期以小时为单位
                        GetDatabase().StringSet(key, Serialize(value), TimeSpan.FromHours(expire));
                    else
                        GetDatabase().StringSet(key, Serialize(value), TimeSpan.FromDays(expire));
                }
                else
                {
                    GetDatabase().StringSet(key, Serialize(value));
                }

            }


            /// <summary>
            /// 判断在缓存中是否存在该key的缓存数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool Exists(string key)
            {
                key = MergeKey(key);
                return GetDatabase().KeyExists(key); //可直接调用
            }

            /// <summary>
            /// 移除指定key的缓存
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool Remove(string key)
            {
                key = MergeKey(key);
                return GetDatabase().KeyDelete(key);
            }

            /// <summary>
            /// 异步设置
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static async Task SetAsync(string key, object value)
            {
                key = MergeKey(key);
                await GetDatabase().StringSetAsync(key, Serialize(value));
            }

            /// <summary>
            /// 根据key获取缓存对象
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<object> GetAsync(string key)
            {
                key = MergeKey(key);
                object value = await GetDatabase().StringGetAsync(key);
                return value;
            }

            /// <summary>
            /// 实现递增
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static long Increment(string key)
            {
                key = MergeKey(key);
                //三种命令模式
                //Sync,同步模式会直接阻塞调用者，但是显然不会阻塞其他线程。
                //Async,异步模式直接走的是Task模型。
                //Fire - and - Forget,就是发送命令，然后完全不关心最终什么时候完成命令操作。
                //即发即弃：通过配置 CommandFlags 来实现即发即弃功能，在该实例中该方法会立即返回，如果是string则返回null 如果是int则返回0.这个操作将会继续在后台运行，一个典型的用法页面计数器的实现：
                return GetDatabase().StringIncrement(key, flags: CommandFlags.FireAndForget);
            }

            /// <summary>
            /// 实现递减
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long Decrement(string key, string value)
            {
                key = MergeKey(key);
                return GetDatabase().HashDecrement(key, value, flags: CommandFlags.FireAndForget);
            }

            /// <summary>
            /// 序列化对象
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            private static byte[] Serialize(object o)
            {
                if (o == null)
                {
                    return null;
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, o);
                    byte[] objectDataAsStream = memoryStream.ToArray();
                    return objectDataAsStream;
                }
            }

            /// <summary>
            /// 反序列化对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="stream"></param>
            /// <returns></returns>
            private static T Deserialize<T>(byte[] stream)
            {
                if (stream == null)
                {
                    return default(T);
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(stream))
                {
                    T result = (T)binaryFormatter.Deserialize(memoryStream);
                    return result;
                }
            }

            /// <summary>
            /// 配置更改时
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
            {
                //LogHelper.SafeLogMessage("Configuration changed: " + e.EndPoint);
            }

            /// <summary>
            /// 发生错误时
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
            {
                //LogHelper.SafeLogMessage("ErrorMessage: " + e.Message);
            }

            /// <summary>
            /// 重新建立连接之前的错误
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
            {
                //LogHelper.SafeLogMessage("ConnectionRestored: " + e.EndPoint);
            }

            /// <summary>
            /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
            {
                //LogHelper.SafeLogMessage("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType +(e.Exception == null ? "" : (", " + e.Exception.Message)));
            }

            /// <summary>
            /// 更改集群
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
            {
                //LogHelper.SafeLogMessage("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
            }

            /// <summary>
            /// redis类库错误
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
            {
                //LogHelper.SafeLogMessage("InternalError:Message" + e.Exception.Message);
            }

            //场景不一样，选择的模式便会不一样，大家可以按照自己系统架构情况合理选择长连接还是Lazy。
            //建立连接后，通过调用ConnectionMultiplexer.GetDatabase 方法返回对 Redis Cache 数据库的引用。从 GetDatabase 方法返回的对象是一个轻量级直通对象，不需要进行存储。

            /// <summary>
            /// 使用的是Lazy，在真正需要连接时创建连接。
            /// 延迟加载技术
            /// 微软azure中的配置 连接模板
            /// </summary>
            //private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            //{
            //    //var options = ConfigurationOptions.Parse(constr);
            //    options.ClientName = GetAppName(); // only known at runtime
            //    //options.AllowAdmin = true;
            //    //return ConnectionMultiplexer.Connect(options);
            //    ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect(Coonstr);
            //    muxer.ConnectionFailed += MuxerConnectionFailed;
            //    muxer.ConnectionRestored += MuxerConnectionRestored;
            //    muxer.ErrorMessage += MuxerErrorMessage;
            //    muxer.ConfigurationChanged += MuxerConfigurationChanged;
            //    muxer.HashSlotMoved += MuxerHashSlotMoved;
            //    muxer.InternalError += MuxerInternalError;
            //    return muxer;
            //});


            #region  当作消息代理中间件使用 一般使用更专业的消息队列来处理这种业务场景

            /// <summary>
            /// 当作消息代理中间件使用
            /// 消息组建中,重要的概念便是生产者,消费者,消息中间件。
            /// </summary>
            /// <param name="channel"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public static long Publish(string channel, string message)
            {
                ISubscriber sub = Instance.GetSubscriber();
                //return sub.Publish("messages", "hello");
                return sub.Publish(channel, message);
            }

            /// <summary>
            /// 在消费者端得到该消息并输出
            /// </summary>
            /// <param name="channelFrom"></param>
            /// <returns></returns>
            public static void Subscribe(string channelFrom)
            {
                ISubscriber sub = Instance.GetSubscriber();
                sub.Subscribe(channelFrom, (channel, message) =>
                {
                    Console.WriteLine((string)message);
                });
            }

            #endregion

            /// <summary>
            /// GetServer方法会接收一个EndPoint类或者一个唯一标识一台服务器的键值对
            /// 有时候需要为单个服务器指定特定的命令
            /// 使用IServer可以使用所有的shell命令，比如：
            /// DateTime lastSave = server.LastSave();
            /// ClientInfo[] clients = server.ClientList();
            /// 如果报错在连接字符串后加 ,allowAdmin=true;
            /// </summary>
            /// <returns></returns>
            public static IServer GetServer(string host, int port)
            {
                IServer server = Instance.GetServer(host, port);
                return server;
            }

            /// <summary>
            /// 获取全部终结点
            /// </summary>
            /// <returns></returns>
            public static EndPoint[] GetEndPoints()
            {
                EndPoint[] endpoints = Instance.GetEndPoints();
                return endpoints;
            }
        }

        #endregion

        private string Encryption(string value, string salt)    //加密
        {
            //将盐混入内容
            string mix = string.Concat(value, salt);
            //加密
            byte[] bytes = Encoding.Default.GetBytes(mix);
            byte[] hash = SHA256.Create().ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}