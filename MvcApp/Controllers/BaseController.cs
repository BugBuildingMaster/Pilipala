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

using RedisHelp;
using MvcThrottle;
using System.Threading;

public class BaseController : Controller
{
    public static RedisHelper RedisHelper = new RedisHelper(0, "127.0.0.1:6379,password=123456,DefaultDatabase=0");      //负责密钥管理的redis数据库
    public static RedisHelper LoginHelper = new RedisHelper(1, "127.0.0.1:6379,password=123456,DefaultDatabase=0");      //负责登录管理的redis数据库
    private static RedisHelper LikeHelper = new RedisHelper(2, "127.0.0.1:6379,password=123456,DefaultDatabase=0");      //负责缓存管理的redis数据库
    public static RedisHelper MailHelper = new RedisHelper(3, "127.0.0.1:6379,password=123456,DefaultDatabase=0");       //负责忘记密码管理的redis数据库
    readonly RecommendManager rManager = new RecommendManager();
    readonly CommunityManager cManager = new CommunityManager();
    readonly EvaluationManager eManager = new EvaluationManager();
    private static LikeNumUpdating rUpdating = new LikeNumUpdating(18000);    //初始化更新监督类

    #region token类
    protected class TokenInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Userphoto { get; set; }
        public string sign { get; set; }
        public DateTime time { get; set; }
        public string pubkey { get; set; }
        public string email { get; set; }
    }

    protected class TokenHelper
    {
        public static string SecretKey { get; set; }//这个服务端加密秘钥 属于私钥
        private static JavaScriptSerializer myJson = new JavaScriptSerializer();

        public static string GenToken(TokenInfo token)  //生成token
        {
            var load = new Dictionary<string, dynamic>
            {
                {"UserId",token.UserId },
                { "UserName",token.UserName },
                { "Userphoto",token.Userphoto },
                { "sign",token.sign },
                {"time",token.time },
                {"pubkey",token.pubkey }
            };
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(load, SecretKey);
        }

        public static string VerEmailToken(TokenInfo token)  //生成邮箱验证token
        {
            var load = new Dictionary<string, dynamic>
            {
                { "email",token.email },
                { "sign",token.sign },
                {"time",token.time },
                {"pubkey",token.pubkey }
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
    protected string gettoken(string UserId, string UserName, string Userphoto, DateTime time, string pubKey, string priKey)
    {
        TokenInfo tokenInfo = new TokenInfo();
        //向token添加信息
        tokenInfo.UserId = UserId;
        tokenInfo.UserName = UserName;
        tokenInfo.Userphoto = Userphoto;
        tokenInfo.time = time;
        //生成关键字段
        string content = UserId + UserName + Userphoto + time.ToString();
        //对关键字段生成签名
        string sign = Sign(content, priKey);
        tokenInfo.sign = sign;
        tokenInfo.pubkey = pubKey;
        TokenHelper.SecretKey = priKey;
        string valueToken = TokenHelper.GenToken(tokenInfo);
        return valueToken;
    }

    //邮箱验证token生成
    protected string getVerToken(string email, DateTime time, string pubKey, string priKey)
    {
        TokenInfo tokenInfo = new TokenInfo();
        //向token添加信息
        tokenInfo.email = email;
        tokenInfo.time = time;
        //生成关键字段
        string content = email + time.ToString();
        //对关键字段生成签名
        string sign = Sign(content, priKey);
        tokenInfo.sign = sign;
        tokenInfo.pubkey = pubKey;
        TokenHelper.SecretKey = priKey;
        string valueToken = TokenHelper.VerEmailToken(tokenInfo);
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
            //检查令牌池中是否含有对应token，没有则删除cookie返回null
            if (LoginHelper.KeyExists(tokenContent))
            {
                //令牌池存在token，根据token获取对应解密私钥进行令牌解密
                priKey = LoginHelper.HashGet(tokenContent, "prikey").ToString();
                var json = decoder.Decode(tokenContent, priKey, true);
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                //校验通过，返回解密后token转为的json对象
                return jo;
            }
            else
            {
                Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                //Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                RedirectToAction("Index", "Animation");
                return null;
            }
        }
        catch (Exception)
        {
            Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
            RedirectToAction("Index", "Animation");
            return null;
        }
    }

    protected JObject readtoken(string tokenContent, bool flag)
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
            //检查令牌池中是否含有对应token，没有则删除cookie返回null
            if (MailHelper.KeyExists(tokenContent))
            {
                //令牌池存在token，根据token获取对应解密私钥进行令牌解密
                priKey = MailHelper.HashGet(tokenContent, "prikey").ToString();
                var json = decoder.Decode(tokenContent, priKey, true);
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                //校验通过，返回解密后token转为的json对象
                return jo;
            }
            else
            {
                Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                //Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                RedirectToAction("Index", "Animation");
                return null;
            }
        }
        catch (Exception)
        {
            Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
            RedirectToAction("Index", "Animation");
            return null;
        }
    }
    #endregion

    #region 验证token
    public bool VerToken(string tokenContent, string pubKey)
    {
        try
        {
            //读取token内容，返回json对象
            JObject list = readtoken(tokenContent);
            //获取签名
            string sign = list["sign"].ToString();
            string name = list["UserName"].ToString();
            string redisName = Encryption(name, "");
            if (LoginHelper.KeyExists(redisName) && LoginHelper.KeyExists(tokenContent))    //检查token是否存在于令牌池，不存在则说明该token已被注销
            {
                pubKey = pubKey.Replace("%0d", "\r").Replace("%0a", "\n");
                //比较token有效时间
                DateTime dateTime = DateTime.Now;
                DateTime Expiration = (DateTime)list["time"];
                Expiration = Expiration.AddDays(1);   //令牌有效时间
                TimeSpan timeSpan = Expiration - dateTime;  //计算时间间隔
                double mineutes = timeSpan.TotalMinutes;
                //拼接字符串生成关键字段
                string content = list["UserId"].ToString() + list["UserName"].ToString() + list["Userphoto"].ToString() + list["time"].ToString();
                //验证token内容是否被篡改
                bool result = verify(content, sign, pubKey);
                if (result == true)
                {
                    //若间隔小于零则已经过期
                    if (mineutes > 0)
                    {
                        //token临近有效期，重新签发token
                        if (mineutes < 30)
                        {
                            //从令牌池中移除对应token
                            LoginHelper.KeyDelete(redisName);
                            LoginHelper.KeyDelete(tokenContent);
                            //根据公钥获取对应私钥
                            string priKey = RedisHelper.HashGet("KeyPool", pubKey);
                            // 更新token并进行发放
                            string newToken = gettoken(list["UserId"].ToString(), list["UserName"].ToString(), list["Userphoto"].ToString(), DateTime.Now, pubKey, priKey);
                            //验证通过后将新token传递给客户端
                            HttpCookie cookie = Request.Cookies["Login"];
                            cookie.Values["Token"] = newToken;
                            cookie.Expires = DateTime.Now.AddHours(1);
                            Response.AppendCookie(cookie);
                            //将新token放入令牌池，作为有效令牌，登录时设置有效期为1小时，键名为用户名，值为签名
                            LoginHelper.HashSet(newToken, "prikey", priKey);
                            LoginHelper.HashSet(newToken, "name", redisName);
                            LoginHelper.KeyExpire(newToken, TimeSpan.FromHours(1));
                            LoginHelper.StringSet(redisName, newToken, TimeSpan.FromHours(1));
                            return true;
                        }
                        else
                            return true;
                    }
                    else
                    {
                        //token过期，验证失败
                        return false;
                    }
                }
                else
                {
                    //信息被篡改，验证失败
                    return false;
                }
            }
            else
            {
                LoginHelper.KeyDelete(redisName);
                LoginHelper.KeyDelete(tokenContent);
                //token已被注销，验证失败
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool VerMailToken(string tokenContent)   //找回密码验证用
    {
        try
        {
            //读取token内容，返回json对象
            JObject list = readtoken(tokenContent, true);
            //获取签名
            string sign = list["sign"].ToString();
            string pubKey = list["pubkey"].ToString();
            if (MailHelper.KeyExists(tokenContent))    //检查token是否存在于令牌池，不存在则说明当前token无效
            {
                //比较token有效时间
                DateTime dateTime = DateTime.Now;
                DateTime Expiration = (DateTime)list["time"];
                Expiration = Expiration.AddMinutes(5);   //令牌有效时间
                TimeSpan timeSpan = Expiration - dateTime;  //计算时间间隔
                double seconds = timeSpan.TotalSeconds;
                //若间隔小于零则已经过期
                if (seconds > 0)
                {
                    //拼接字符串生成关键字段
                    string content = list["email"].ToString() + list["time"].ToString();
                    //验证token内容是否被篡改
                    bool result = verify(content, sign, pubKey);
                    if (result == true)
                        return true;
                    else
                        return false;   //信息被篡改，验证失败
                }
                else
                    return false;   //token过期，验证失败
            }
            else
            {
                MailHelper.KeyDelete(tokenContent);
                //token已被注销，验证失败
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

    #region 加密
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
    #endregion

    #region 添加历史记录
    //添加历史记录
    [EnableThrottling(PerSecond = 2, PerMinute = 40)]
    public ActionResult AddWatch(string type, int id)
    {

        if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
        {
            return Json("login", JsonRequestBehavior.AllowGet);
        }
        else
        {
            HttpCookie cookie = Request.Cookies["Login"];
            string tokenContent = cookie.Values["Token"];
            string pubKey = Request.Cookies["Key"].Value;
            if (VerToken(tokenContent, pubKey))
            {
                JObject name = readtoken(cookie.Values["Token"]);
                rManager.AddWatch(type, id, name["UserName"].ToString());
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("login", JsonRequestBehavior.AllowGet);
            }
        }
    }
    #endregion

    #region Redis点赞缓存池

    #region 更新监督类
    private class LikeNumUpdating       //更新监督类
    {
        private static int num = 0;     //设定的更新界限
        private static int count = 0;   //计数属性
        private static int timespan;    //更新间隔
        System.Threading.Timer timer;   //更新计时器

        public delegate string update(object sender, EventArgs e);  //声明委托
        public event update onupdate;       //声明委托的事件

        public LikeNumUpdating(int? interval)   //初始化监督类，根据输入时间初始化计时器
        {
            interval = interval ?? 1800000;
            timespan = (int)interval;
            timer = new System.Threading.Timer(new TimerCallback(timerChange), null, timespan, timespan);
        }
        private void timerChange(object sender)   //计时器所触发的更新
        {
            onupdate(this, null);
        }
        private void change()       //计数属性所触发的更新
        {
            if (onupdate != null)
            {
                onupdate(this, null);
                timer.Change(timespan, timespan);
            }
        }

        public bool isInit { get; set; }    //指示初始化状态
        public bool IsUpdating { get; set; }    //指示更新状态
        public int state        //通过state修改内部更新界限
        {
            get { return num; }
            set { num = value; }
        }

        public int Count        //通过Count修改内部计数
        {
            get { return count; }
            set
            {
                count = value;
                if (count > num)
                {
                    change();   //当计数达到设定界限时，触发更新并将计数置零
                    count = 0;
                }
            }
        }
    }

    #endregion

    #region 操作字典
    private class HandleList        //操作字典
    {
        public string name { get; set; }    //操作用户的姓名
        public int id { get; set; }         //操作对象的id
        public DateTime time { get; set; }  //操作时间
        public string type { get; set; }    //记录所需要操作的类型
    }

    #endregion

    #region 数据库同步
    private string update(object sender, EventArgs e)       //redis向数据库同步数据
    {
        try
        {
            if (rUpdating.IsUpdating)     //保证更新时函数不会重复激活
                return "updating";
            rUpdating.IsUpdating = true;
            //设置所有更新前缀
            string[] updatelist = new string[]
            {"ShortComment","Dongtai","DongtaiComment","EvaluationLike","EvaluationDisLike"};
            foreach (var name in updatelist)    //历遍需要同步的类别
            {
                List<string> list = new List<string>();
                List<string> keylist = new List<string>();
                string[] value = LikeHelper.ToGet(name + "*_mid_[0-9]*end");   //操作名
                string[] key = LikeHelper.ToGet(name + "[0-9]*end");       //数据名
                List<HandleList> dealList = new List<HandleList>();
                foreach (var item in key)
                {
                    keylist.Add(item);  //创建数据名删除列表
                }
                foreach (var item in value)
                {
                    list.Add(item); //创建删除列表
                    string[] attr = item.Split(new string[] { "_mid_" }, StringSplitOptions.None);    //将获得的键值进行过滤
                    HandleList temp = new HandleList   //过滤字符串形成操作字典
                    {
                        name = attr[0].Remove(0, name.Length),
                        id = Convert.ToInt32(attr[1].Substring(0, attr[1].Length - 3)),
                        time = Convert.ToDateTime(LikeHelper.StringGet(item))
                    };
                    dealList.Add(temp);
                }
                switch (name)
                {
                    case "ShortComment":
                        {
                            foreach (var item in dealList)
                            {
                                cManager.ShortCommentAddLike(item.id, item.name, item.time); //短评修改
                            }
                            break;
                        }
                    case "Dongtai":
                        {
                            foreach (var item in dealList)
                            {
                                cManager.AddLike(item.id, item.name, item.time); //动态修改
                            }
                            break;
                        }
                    case "DongtaiComment":
                        {
                            foreach (var item in dealList)
                            {
                                cManager.DongtaiCommentAddLike(item.id, item.name, item.time); //动态评论修改
                            }
                            break;
                        }
                    case "EvaluationLike":
                        {
                            foreach (var item in dealList)
                            {
                                eManager.AddLikeOrDislike(2, item.id, item.name, item.time); //测评点赞修改
                            }
                            break;
                        }
                    case "EvaluationDisLike":
                        {
                            foreach (var item in dealList)
                            {
                                eManager.AddLikeOrDislike(3, item.id, item.name, item.time); //测评点踩修改
                            }
                            break;
                        }
                }
                LikeHelper.KeyDelete(list);    //清空更新完成的操作记录
                LikeHelper.KeyDelete(keylist); //清空缓存的对象点赞数记录(防止脏读)
            }
            rUpdating.IsUpdating = false;
            return "success";
        }
        catch (Exception)
        {
            return "error";
        }
    }

    #endregion

    #region 初始化更新监督类与操作字典
    private List<HandleList> tempHandle = new List<HandleList>();
    private int tempCount = 0;

    public void InitUpdater()
    {
        if (rUpdating.isInit != true)
        {
            rUpdating.state = 300;           //设置更新限度
            rUpdating.Count = 0;            //初始化计数
            rUpdating.onupdate += update;   //附加委托函数
            rUpdating.IsUpdating = false;   //初始化更新状态
            rUpdating.isInit = true;        //将初始化标记置真
        }
    }
    #endregion

    #region Redis点赞缓存
    protected string ToLike(int id, string name, string type, DateTime time, bool isLoop)       //缓存更新函数
    {
        if (rUpdating.isInit != true)
        {
            InitUpdater();
        }
        bool exists = cManager.LikeExist(id, name, type);   //判断数据库是否存在记录
        string pattern;     //操作名
        string value = type + id + "end";       //数据名
        pattern = type + name + "_mid_" + id.ToString() + "end";        //拼接key
        if (isLoop)     //判断是否正在进行补充操作，如果是则不进行计数相关操作
        {
            if (rUpdating.Count >= 30)
            {
                string flag = update(this, null);
                if (flag == "updating")  //正在更新，将需要插入的数据存入临时对象，等待更新完成继续存入
                {
                    HandleList temp = new HandleList
                    {
                        id = id,
                        name = name,
                        time = DateTime.Now,
                        type = type
                    };
                    tempHandle.Add(temp);
                    tempCount += 1;
                    if (LikeHelper.KeyExists(value))
                        return LikeHelper.StringGet(value);
                    else
                        return cManager.GetNumber(id, type).ToString();
                }
                else if (flag == "success")     //更新完成
                {
                    if (tempCount != 0)     //更新期间仍有操作，将临时存储的数据输入进行操作
                    {
                        foreach (var item in tempHandle)
                        {
                            ToLike(item.id, item.name, item.type, item.time, true);
                        }
                        tempCount = 0;      //操作完成，将临时存储对象清空初始化
                        tempHandle.Clear();
                    }
                    rUpdating.Count = 0;
                }
                else
                    return "error";
            }
        }
        //确定在当前数据库中对应的操作
        if (exists)
        {
            if (LikeHelper.KeyExists(pattern))       //判断缓存池是否存在对应记录
            {
                LikeHelper.KeyDelete(pattern);       //缓存池中存在该操作记录，所以删除该操作
                if (!LikeHelper.KeyExists(value))    //判断池中是否存有对应数据的点赞数
                {
                    //不存在对应数据的点赞数，进行建立
                    int num = Convert.ToInt32(cManager.GetNumber(id, type).ToString());  //获取当前数据的点赞数并保存
                    LikeHelper.StringSet(value, num, TimeSpan.FromMinutes(30));  //保存点赞数
                }
                LikeHelper.StringIncrement(value); //为当前操作点赞数加一
            }
            else
            {
                LikeHelper.StringSet(pattern, time.ToString());       //缓存池中不存在该操作记录，所以添加该操作
                if (!LikeHelper.KeyExists(value))            //判断池中是否存有对应数据的点赞数
                {
                    int num = Convert.ToInt32(cManager.GetNumber(id, type).ToString());  //获取当前数据的点赞数并保存
                    LikeHelper.StringSet(value, num, TimeSpan.FromMinutes(30));  //保存点赞数
                }
                LikeHelper.StringDecrement(value); //为当前操作点赞数减一
            }
        }
        else
        {
            if (LikeHelper.KeyExists(pattern))       //判断缓存池是否存在对应记录
            {
                LikeHelper.KeyDelete(pattern);       //缓存池中存在该操作记录，所以删除该操作
                if (!LikeHelper.KeyExists(value))    //判断池中是否存有对应数据的点赞数
                {
                    //不存在对应数据的点赞数，进行建立
                    int num = Convert.ToInt32(cManager.GetNumber(id, type).ToString());  //获取当前数据的点赞数并保存
                    LikeHelper.StringSet(value, num, TimeSpan.FromMinutes(30));  //保存点赞数
                }
                LikeHelper.StringDecrement(value); //为当前操作点赞数减一
            }
            else
            {
                LikeHelper.StringSet(pattern, time.ToString());       //缓存池中不存在该操作记录，所以添加该操作
                if (!LikeHelper.KeyExists(value))            //判断池中是否存有对应数据的点赞数
                {
                    int num = Convert.ToInt32(cManager.GetNumber(id, type).ToString());  //获取当前数据的点赞数并保存
                    LikeHelper.StringSet(value, num, TimeSpan.FromMinutes(30));  //保存点赞数
                }
                LikeHelper.StringIncrement(value); //为当前操作点赞数加一
            }
        }
        if (!isLoop)
        {
            rUpdating.Count += 1;
            if (!LikeHelper.KeyExists(value))    //判断池中是否存有对应数据的点赞数
            {
                //不存在对应数据的点赞数，进行建立
                int num = Convert.ToInt32(cManager.GetNumber(id, type).ToString());  //获取当前数据的点赞数并保存
                LikeHelper.StringSet(value, num, TimeSpan.FromMinutes(30));  //保存点赞数
            }
        }
        return LikeHelper.StringGet(value);
    }

    #endregion

    #endregion
}