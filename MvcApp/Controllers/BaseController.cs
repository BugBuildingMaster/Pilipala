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
            string sign = Sign(content, Session["Private"].ToString());
            tokenInfo.sign = sign;
            TokenHelper.SecretKey = Session["Private"].ToString();
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
            string priKey = Session["Private"].ToString();
            var json = decoder.Decode(tokenContent, priKey, true);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            //校验通过，返回解密后的json对象
            return jo;
        }
        #endregion

        #region 验证token
        protected bool VerToken(string tokenContent, string pubKey)
        {
            //读取token内容，返回json对象
            JObject list = readtoken(tokenContent);
            //比较token有效时间
            DateTime dateTime = DateTime.Now;
            TimeSpan timeSpan = dateTime - (DateTime)list["time"];
            double hours = timeSpan.TotalHours;
            if (hours < 1)
            {
                if (hours > 0)
                {
                    //移除当前token
                    Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
                    // 更新token
                    string newToken = gettoken(list["UserId"].ToString(), list["UserName"].ToString(), list["Userphoto"].ToString(), DateTime.Now);
                    HttpCookie cookie = new HttpCookie("Login");
                    cookie.Values.Add("Token", newToken);
                    cookie.Expires = DateTime.Now.AddHours(1);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    //token过期，验证失败
                    return false;
                }
            }
            //拼接字符串生成关键字段
            string content = list["UserId"].ToString() + list["UserName"].ToString() + list["Userphoto"].ToString() + list["time"].ToString();
            //获取签名
            string sign = list["sign"].ToString();
            //验证token内容是否被篡改
            bool result = verify(content, sign, pubKey);
            return result;
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
    }
}