﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Models;
using BLL;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using Org.BouncyCastle;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
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
using MvcThrottle;

namespace MvcApp.Controllers
{
    public class UsersController : BaseController
    {
        readonly UsersManager usersManager = new UsersManager();
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录视图
        /// </summary>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 4, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 用户注册视图
        /// </summary>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 4, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Register()
        {
            return View();
        }

        #region 验证用户名唯一
        [HttpGet]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string IsUsernameUnique(string name)
        {
            if (usersManager.IsUsernameUnique(name))
                return "no";
            else
                return "yes";
        }
        #endregion


        /*----------------------------------------------------------------------------*/

        #region 注销登录
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string LoginOut()
        {
            string data;
            try
            {
                //从令牌池中移除对应令牌
                HttpCookie cookie = Request.Cookies["Login"];
                string name = readtoken(cookie.Values["Token"])["UserName"].ToString();
                string redisName = Encryption(name, "");
                string token = cookie.Values["Token"];
                RedisHelper.KeyDelete(redisName);
                RedisHelper.KeyDelete(token);
                Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                data = "已注销登录！";
                return data;
            }
            catch (Exception)
            {
                data = "出现错误！";
                return data;
            }
        }
        #endregion

        #region 验证邮箱唯一
        [HttpGet]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string IsEmailUnique(string email)
        {
            if (usersManager.IsEmailUnique(email))
                return "no";
            else
                return "yes";
        }
        #endregion

        #region 生成公钥私钥
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string setRsa()      //初始化，生成PKCS1密钥对，将私钥保存在session中用于解密接收到的密码密文，将公钥传到前端用于密码加密
        {
            if (RedisHelper.KeyExists("KeyPool") == false)
                RedisHelper.StringSet("keyNum", 0);
            if (RedisHelper.KeyExists("keyNum"))
            {
                //当维护的密钥池数量小于50时，继续生成密钥对
                if (Convert.ToInt32(RedisHelper.StringGet("keyNum")) < 50)
                {
                    RedisHelper.StringIncrement("keyNum");  //计数加一
                    RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();
                    //添加参数
                    RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new SecureRandom(), 1024, 25);
                    //初始化构造器
                    keyGenerator.Init(param);
                    //生成密钥对
                    AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
                    TextWriter textWriter = new StringWriter();
                    //获取私钥
                    PemWriter pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(keyPair.Private); //获取密钥对的私钥
                    pemWriter.Writer.Flush();   //清空缓存区，防止脏写
                    string priKey = textWriter.ToString();
                    priKey = RsaKeyConvert.PrivateKeyPkcs1ToPkcs8(priKey);
                    //获取公钥
                    TextWriter textWriter1 = new StringWriter();
                    PemWriter pemWriter1 = new PemWriter(textWriter1);
                    pemWriter1.WriteObject(keyPair.Public); //获取密钥对的公钥
                    pemWriter1.Writer.Flush();   //清空缓存区，防止脏写
                    string pubKey = textWriter1.ToString();
                    //将密钥对存入redis中的密钥对集合
                    //pubKey = pubKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    //priKey = priKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    RedisHelper.HashSet("KeyPool", pubKey, priKey);
                    RedisHelper.KeyExpire("KeyPool", TimeSpan.FromDays(1));
                    //将公钥存入cookie方便读取
                    Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["Key"].Value = pubKey;
                    Response.Cookies["Key"].Expires = DateTime.Now.AddDays(1);
                    return pubKey;  //将公钥传到前端
                }
                else
                {
                    //生成的密钥对数量足够，从密钥对池中随机取出公钥进行传递
                    Dictionary<string, string> keyPairList = RedisHelper.HashGetKeys<string>("KeyPool");
                    List<string> keyList = new List<string>(keyPairList.Keys);
                    Random ran = new Random();
                    string pubKey = keyList[ran.Next(keyList.Count)].ToString();
                    //将公钥存入cookie方便读取
                    Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["Key"].Value = pubKey;
                    Response.Cookies["Key"].Expires = DateTime.Now.AddDays(1);
                    return pubKey;
                }

            }
            else
            {
                RedisHelper.StringSet("keyNum", 0);
                setRsa();
                return null;
            }

        }
        #endregion

        #region 密钥处理
        //使用私钥解密密文
        protected static string DecryptData(string key, string data)
        {
            key = key.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            key = key.Replace("-----BEGINPRIVATEKEY-----", "").Replace("-----ENDPRIVATEKEY-----", "");
            //加载解密数据
            byte[] valueByte = Convert.FromBase64String(data);
            string result = "";
            for (int j = 0; j < valueByte.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {

                    buf[i] = valueByte[i + 128 * j];
                }
                result += decrypt(buf, key);
            }
            return result;
        }

        protected static string decrypt(byte[] data, string privateKey)
        {
            string result = "";
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] source = rsa.Decrypt(data, false);
            char[] asciiChars = new char[Encoding.GetEncoding("UTF-8").GetCharCount(source, 0, source.Length)];
            Encoding.GetEncoding("UTF-8").GetChars(source, 0, source.Length, asciiChars, 0);
            result = new string(asciiChars);
            return result;
        }

        protected static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {

            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        protected static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemstr);
            if (pkcs8privatekey != null)
            {
                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
                return rsa;
            }
            else
                return null;
        }

        protected static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);
                if (!CompareBytearrays(seq, SeqOID))
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)
                    return null;

                bt = binr.ReadByte();
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }
        }

        protected static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        protected static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        protected static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
        #endregion

        #region 注册
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string Register(string username, string password, string email)
        {
            try
            {
                //解密
                password = password.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                string pubKey = Request.Cookies["Key"].Value;
                //前后端转义字符转码
                pubKey = pubKey.Replace("%0d", "\r").Replace("%0a", "\n");
                string priKey = RedisHelper.HashGet("KeyPool", pubKey);
                //使用私钥解密
                string trueValue = DecryptData(priKey, password);
                //生成新盐
                string salt = CreateSalt(20);
                //将新盐与密码进行拼接并加密
                string pwd = Encryption(trueValue, salt);
                string v = usersManager.Register(username, pwd, email, salt);
                if (v == "illegalname")
                {
                    return "illegalname";
                }
                else if (v == "success")
                {
                    return "success";
                }
                else if (v == "illEmail")
                {
                    return "illEmail";
                }
                else
                {
                    return "fail";
                }
            }
            catch (Exception)
            {
                return "fail";
            }
        }
        #endregion

        #region 用户登录
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string Login(string username, string password)
        {
            try
            {
                //检查是否存在该用户
                string exists = IsUsernameUnique(username);
                if (exists == "yes")
                {
                    return "fail";
                }
                password = password.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                //获取公钥
                string pubKey = Request.Cookies["Key"].Value;
                //前后端转义字符转码
                pubKey = pubKey.Replace("%0d", "\r").Replace("%0a", "\n");
                //根据公钥从密钥对池中提取出对应私钥
                string priKey = RedisHelper.HashGet("KeyPool", pubKey);
                //使用私钥解密
                string trueValue = DecryptData(priKey, password);
                string salt = usersManager.GetSalt(username);
                if (usersManager.ComparePwd(Encryption(trueValue, salt), username))
                {
                    Users user = usersManager.GetUser(username);
                    //将用户名加密后作为令牌池的key
                    string redisName = Encryption(user.UserName, "");
                    //如果令牌池已存在该用户则移除token重新生成(单用户登录)
                    if (RedisHelper.KeyExists(redisName))
                    {
                        string OldToken = RedisHelper.StringGet(redisName);
                        RedisHelper.KeyDelete(redisName);
                        RedisHelper.KeyDelete(OldToken);
                    }
                    //生成token
                    string token = gettoken(user.Userid.ToString(), user.UserName, user.UsersInfo.Portrait, DateTime.Now, pubKey, priKey);
                    //将token传递到前端
                    HttpCookie cookie = new HttpCookie("Login");
                    cookie.Values.Add("Token", token);
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie);
                    //将token放入令牌池，作为有效令牌，登录时设置有效期为1天
                    //添加一个hash表，名称为token，内部放入两个键值对
                    //一个键名为prikey，值为私钥；另一个键名为name，值为加密后的用户名
                    RedisHelper.HashSet(token, "prikey", priKey);
                    RedisHelper.HashSet(token, "name", redisName);
                    RedisHelper.KeyExpire(token, TimeSpan.FromDays(1));
                    RedisHelper.StringSet(redisName, token, TimeSpan.FromDays(1));
                    return "success";
                }
                else return "fail";
            }
            catch (Exception)
            {
                return "error";
            }

        }
        #endregion

        #region 修改密码
        //修改密码
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public string EditPassword(string oldPwd, string newPwd)
        {
            try
            {
                //获取用户的盐
                HttpCookie cookie = Request.Cookies["Login"];
                JObject username = readtoken(cookie.Values["Token"]);
                string name = username["UserName"].ToString();
                string salt = usersManager.GetSalt(name);
                //验证旧密码是否正确
                if (usersManager.ComparePwd(Encryption(oldPwd, salt), name))
                {
                    //验证新密码是否与旧密码一样
                    if (Encryption(oldPwd, salt) == Encryption(newPwd, salt))
                    {
                        return "re";
                    }
                    else
                    {
                        string thePast = Encryption(oldPwd, salt);
                        string newSalt = CreateSalt(20);
                        string theNew = Encryption(newPwd, newSalt);
                        return usersManager.EditPassword(thePast, theNew, name, newSalt);
                    }
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception)
            {
                return "fail";
            }

        }
        #endregion

        #region 盐加密与生成
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

        //强随机盐生成
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        //生成盐
        private static string CreateSalt(int stringLength)
        {
            //字典
            string key = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int length = key.Length;
            StringBuilder Salt = new StringBuilder(length);

            for (int i = 0; i < stringLength; ++i)
            {
                //按每位生成
                Salt.Append(key[SetRandomSeeds(length)]);
            }
            return Salt.ToString();
        }

        //生成强随机位
        private static int SetRandomSeeds(int length)
        {
            decimal maxValue = (decimal)long.MaxValue;
            byte[] array = new byte[8];
            rng.GetBytes(array);

            return (int)(Math.Abs(BitConverter.ToInt64(array, 0)) / maxValue * length);
        }
        #endregion

        #region test
        /*//测试Redis用
        [HttpPost]
        public string test(string key)
        {
            try
            {
                return RedisHelper.HashGet("KeyPool", key);
            }
            catch (Exception)
            {

                return "no";
            }
        }

        [HttpPost]
        public JObject test3(string key)
        {
            try
            {
                JObject result = new JObject();//假设result为数据结构
                Random ran = new Random();
                Dictionary<string, string> choose = RedisHelper.HashGetKeys<string>(key);
                List<string> keyList = new List<string>(choose.Keys);
                string o = keyList[ran.Next(keyList.Count)].ToString();
                string list = JsonConvert.SerializeObject(RedisHelper.HashGetKeys<string>(key));
                result = (JObject)JsonConvert.DeserializeObject(list);
                return result;
            }
            catch (Exception)
            {

                return (JObject)"no";
            }
        }*/

        /*[HttpPost]
        public string test1(string key)
        {
            try
            {
                if (RedisHelper.Exists(key))
                {
                    string a = RedisHelper.Get(key).ToString();
                    return a;
                }
                else
                {
                    return "已过期或不存在";
                }
            }
            catch (Exception)
            {

                return "no";
            }
        }

        [HttpPost]
        public string test2(string key)
        {
            try
            {
                if (RedisHelper.Exists(key))
                {
                    return RedisHelper.Type(key);
                    RedisHelper.Remove(key).ToString();
                    return "ok";
                }
                else
                {
                    return "已过期或不存在";
                }
            }
            catch (Exception)
            {

                return "no";
            }
        }*/

        /*//签名测试用
        [HttpGet]        //生成并返回密钥对
        public ActionResult getRsa()
        {
            string[] list = new string[4];
            list[0] = setRsa();
            list[1] = Session["Private"].ToString().Replace("\r", "").Replace("\n", "").Replace(" ", "");
            list[2] = list[0].Replace("\r", "").Replace("\n", "").Replace(" ", "");
            list[3] = Session["Private"].ToString();
            return Json(new { pubKey = list[0], priKey = list[1], a = list[2], b = list[3] }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]       //生成token
        public string gettoken(string UserId, string UserName, string Userphoto)
        {
            TokenInfo tokenInfo = new TokenInfo();
            //向token添加信息
            tokenInfo.UserId = UserId;
            tokenInfo.UserName = UserName;
            tokenInfo.Userphoto = Userphoto;
            //关键字段
            string content = UserId + UserName + Userphoto;
        
            Session["thing"] = content;    //将私钥保存在session中
        
            //对关键字段生成签名
            string sign = Sign(Session["thing"].ToString(), Session["Private"].ToString());
            tokenInfo.sign = sign;
        
            Session["Sign"] = sign;    //将私钥保存在session中
        
            TokenHelper.SecretKey = Session["Private"].ToString();
            string valueToken = TokenHelper.GenToken(tokenInfo);
            return valueToken;
        }
        [HttpPost]       //读取token
        public string readtoken(string tokenContent)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtAlgorithm alg = new HMACSHA256Algorithm();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, alg);
            var json = decoder.Decode(tokenContent, Session["Private"].ToString(), true);
            //校验通过，返回解密后的字符串
            return json;
        }
        [HttpPost]       //验证签名
        public bool Ver(string content, string sign)
        {
            bool result = verify(content, sign, Session["pub"].ToString());
            return result;
        }
        */
        #endregion

    }
}
