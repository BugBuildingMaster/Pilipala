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

public class BaseController : Controller
{
    public static RedisHelper RedisHelper = new RedisHelper(0, "127.0.0.1:6379,password=123456,DefaultDatabase=0");
    readonly RecommendManager rManager = new RecommendManager();

    #region token类
    protected class TokenInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Userphoto { get; set; }
        public string sign { get; set; }
        public DateTime time { get; set; }
        public string pubkey { get; set; }
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
            if (RedisHelper.KeyExists(tokenContent))
            {
                //令牌池存在token，根据token获取对应解密私钥进行令牌解密
                priKey = RedisHelper.HashGet(tokenContent, "prikey").ToString();
                var json = decoder.Decode(tokenContent, priKey, true);
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                //校验通过，返回解密后token转为的json对象
                return jo;
            }
            else
            {
                Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Key"].Expires = DateTime.Now.AddDays(-1);
                RedirectToAction("Index", "Animation");
                return null;
            }
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
            if (RedisHelper.KeyExists(redisName) && RedisHelper.KeyExists(tokenContent))    //检查token是否存在于令牌池，不存在则说明该token已被注销
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
                            RedisHelper.KeyDelete(redisName);
                            RedisHelper.KeyDelete(tokenContent);
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
                            RedisHelper.HashSet(newToken, "prikey", priKey);
                            RedisHelper.HashSet(newToken, "name", redisName);
                            RedisHelper.KeyExpire(newToken, TimeSpan.FromDays(1));
                            RedisHelper.StringSet(redisName, newToken, TimeSpan.FromDays(1));
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
                RedisHelper.KeyDelete(redisName);
                RedisHelper.KeyDelete(tokenContent);
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

}