﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;
using Newtonsoft.Json.Linq;
using PagedList;
using MvcThrottle;

namespace MvcApp.Controllers
{
    public class TouristController : BaseController
    {
        readonly UsersManager uManager = new UsersManager();

        // GET: Tourist
        //游客视角个人中心
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult TouristCenter(int? id, string name)
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject username = readtoken(cookie.Values["Token"]);
                    id = id ?? Convert.ToInt32(username["UserId"]);
                    var visitor = username["UserName"].ToString();
                    if (visitor == name || id == Convert.ToInt32(username["UserId"]))
                    {
                        return RedirectToAction("Center", "User", new { id });
                    }
                    else
                    {
                        InitUpdater();
                        ViewBag.username = name;
                        ViewBag.userid = id;
                        ViewBag.visitor = visitor;
                        ViewBag.visitorid = Convert.ToInt32(username["UserId"]);
                        UsersInfo user = uManager.GetUsersInfo((int)id);
                        return View(user);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
            }
        }

        //关注列表
        [HttpGet]
        public JsonResult TouristFollow(int page, string name)
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject username = readtoken(cookie.Values["Token"]);
            IEnumerable<tempFollow> follows = uManager.GetTouristFollow(page, name, username["UserName"].ToString());
            return Json(follows, JsonRequestBehavior.AllowGet);
        }

        //粉丝列表
        [HttpGet]
        public JsonResult TouristFans(int page, string name)
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject username = readtoken(cookie.Values["Token"]);
            IEnumerable<tempFans> fans = uManager.GetTouristFans(page, name, username["UserName"].ToString());
            return Json(fans, JsonRequestBehavior.AllowGet);
        }

        //游客发布测评分布视图
        public ActionResult TouristEvaluation(string name)
        {
            return PartialView(uManager.GetTouristEvaluations(name));
        }

        //游客分布视图
        public ActionResult TouristShortComment(string name)
        {
            return PartialView(uManager.GetTouristShortComment(name));
        }

        //游客分布视图
        public ActionResult TouristDynamic(string name)
        {
            return PartialView(uManager.GetTouristDongtai(name));
        }
    }
}