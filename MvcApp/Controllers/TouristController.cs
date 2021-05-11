using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;
using Newtonsoft.Json.Linq;
using PagedList;

namespace MvcApp.Controllers
{
    public class TouristController : BaseController
    {
        readonly UsersManager uManager = new UsersManager();

        // GET: Tourist
        //游客视角个人中心
        public ActionResult TouristCenter(int? id, string name)
        {
            if (Request.Cookies["Login"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject username = readtoken(cookie.Values["Token"]);
                var visitor = username["UserName"].ToString();
                if (visitor == name)
                {
                    return RedirectToAction("Center", "User", new { id });
                }
                else
                {
                    ViewBag.username = name;
                    ViewBag.userid = id;
                    UsersInfo user = uManager.GetUsersInfo((int)id);
                    return View(user);
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