﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using Models;
using Newtonsoft.Json.Linq;

namespace MvcApp.Controllers
{
    public class CommunityController : BaseController
    {

        readonly CommunityManager cManager = new CommunityManager();

        // GET: Community
        public ActionResult Index()
        {
            if (Request.Cookies["Login"] != null && Request.Cookies["Key"] != null)
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                bool flag = VerToken(tokenContent, pubKey);
                if (flag == false)
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    //获取token中的名字
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.userid = name["UserId"].ToString();
                    cManager.Dynamic(name["UserName"].ToString());   //社区初始化
                    return View();
                }
            }
            else
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
        }

        //动态详情页
        public ActionResult DynamicDetail(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.userid = name["UserId"].ToString();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }
            return View(cManager.DynamicDetail(id));
        }

        //全部动态分布视图
        public ActionResult AllDynamic()
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.username = name["UserName"].ToString();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }
            return PartialView(cManager.AllDynamic());
        }

        //动态评论分布视图
        public ActionResult DynamicComment(int? id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.username = name["UserName"].ToString();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }
            return PartialView(cManager.DynamicComment((int)id));
        }

        //评论回复分布视图
        public ActionResult DynamicReply(int? id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.username = name["UserName"].ToString();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }
            return PartialView(cManager.DynamicReply((int)id));
        }

        //全部测评分布视图
        public ActionResult AllEvaluation()
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.username = name["UserName"].ToString();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }
            return PartialView(cManager.AllEvaluation());
        }

        //全部短评分布视图
        public ActionResult AllShortComment()
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            HttpCookie cookie = Request.Cookies["Login"];
            string tokenContent = cookie.Values["Token"];
            string pubKey = Request.Cookies["Key"].Value;
            if (VerToken(tokenContent, pubKey))
            {
                return PartialView(cManager.AllShortComment());
            }
            return RedirectToAction("Login", "Users");
        }

        //返回用户头像地址
        public ActionResult ThePortrait()
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("https://hbimg.huabanimg.com/0cd238587a0984d24b8688ad35c187da3ace5314317c-KPcKiS_fw658/format/webp");

            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    var Portrait = cManager.GetPortrait(name["UserName"].ToString());
                    return Content(Portrait.ToString());
                }
                else
                {
                    return Content("https://hbimg.huabanimg.com/0cd238587a0984d24b8688ad35c187da3ace5314317c-KPcKiS_fw658/format/webp");
                }
            }
        }

        //返回指定用户头像地址
        public ActionResult GetPortrait(string name)
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("https://hbimg.huabanimg.com/0cd238587a0984d24b8688ad35c187da3ace5314317c-KPcKiS_fw658/format/webp");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    var Portrait = cManager.GetPortrait(name);
                    return Content(Portrait.ToString());
                }
                else
                {
                    return Content("https://hbimg.huabanimg.com/0cd238587a0984d24b8688ad35c187da3ace5314317c-KPcKiS_fw658/format/webp");
                }
            }
        }

        //返回指定用户id
        public ActionResult GetID(string name)
        {
            return Content(cManager.GetID(name));
        }

        //添加动态
        [HttpPost]
        public ActionResult AddDynamic(string content)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    bool flag = cManager.AddDynamic(content, name["UserName"].ToString());
                    if (flag)
                    {
                        var dynamic = cManager.AllDynamic();
                        return PartialView("AllDynamic", dynamic);
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }
        //添加评论
        [HttpPost]
        public ActionResult AddComment(int dtid, string content)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    bool flag = cManager.AddComment(dtid, content, name["UserName"].ToString());
                    if (flag)
                    {
                        var comm = cManager.DynamicComment(dtid);
                        return PartialView("DynamicComment", comm);
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }
        //动态评论回复添加函数
        [HttpPost]
        public ActionResult AddCommentReply(int id, string content, int dtid)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    bool flag = cManager.AddCommentReply(id, content, dtid, name["UserName"].ToString());
                    if (flag)
                    {
                        var comm = cManager.DynamicComment(dtid);
                        return PartialView("DynamicComment", comm);
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }

        //动态删除
        [HttpPost]
        public JsonResult DeleteDongtai(int id)
        {
            bool flag = cManager.DeleteDynamic(id);
            if (flag)
            {
                return Json("");
            }
            else
            {
                return Json("Fail");
            }
        }
        //删除评论
        [HttpPost]
        public ActionResult DeleteComment(int dtid)
        {
            bool flag = cManager.DeleteComment(dtid);
            if (flag)
            {
                return Content("Success");
            }
            else
            {
                return Content("Fail");
            }
        }
        //删除评论回复
        [HttpPost]
        public ActionResult DeleteReply(int dtid)
        {
            bool flag = cManager.DeleteReply(dtid);
            if (flag)
            {
                return Content("Success");
            }
            else
            {
                return Content("Fail");
            }
        }

        //动态点赞
        [HttpPost]
        public ActionResult AddLike(int id)
        {

            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    string i = cManager.AddLike(id, name["UserName"].ToString());
                    return Content(i);
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }

        //短评点赞
        [HttpPost]
        public ActionResult ShortCommentAddLike(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    string i = cManager.ShortCommentAddLike(id, name["UserName"].ToString());
                    return Content(i);
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }

        //动态评论点赞
        [HttpPost]
        public ActionResult DongtaiCommentAddLike(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    string i = cManager.DongtaiCommentAddLike(id, name["UserName"].ToString());
                    return Content(i);
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }

        //动态评论数返回
        [HttpPost]
        public ActionResult ShortCommentNum(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    string i = cManager.ShortCommentNum(id);
                    return Content(i);
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return Content("login");
                }
            }
        }
    }
}