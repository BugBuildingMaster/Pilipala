using System;
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
    public class SearchController : BaseController
    {
        readonly SearchManager sManager = new SearchManager();

        // 初始搜索界面
        public ActionResult OriginSearch()
        {
            return View();
        }

        //搜索功能
        [HttpGet]
        public ActionResult Search(string KeyWord)
        {
            string name;
            if (KeyWord == null)
            {
                return RedirectToAction("OriginSearch", "Search");
            }
            else
            {
                if (Request.Cookies["Login"] == null)        //未登录时的情况
                {
                    name = "";
                }
                else
                {
                    HttpCookie cookie = Request.Cookies["Login"];
                    JObject username = readtoken(cookie.Values["Token"]);
                    name = username["UserName"].ToString();
                    ViewBag.username = name;
                    ViewBag.userid = username["UserId"].ToString();
                }
                sManager.Search(KeyWord, name);
                return View();
            }  
        }

        //获取测评信息
        [HttpGet]
        public JsonResult GetEvaluation(int id, int type)
        {
            var theLongth = 300;
            if (type == 1)          //长串
            {
                string evaluation = sManager.GetEvaluationPreview(id);
                if (evaluation.Length < 300)
                {
                    theLongth = evaluation.Length;
                    evaluation = "";
                }
                else
                {
                    evaluation = evaluation.Substring(theLongth, evaluation.Length - 300);
                }
                return Json(evaluation, JsonRequestBehavior.AllowGet);
            }
            else                    //短串
            {
                string evaluation = sManager.GetEvaluationPreview(id);
                if (evaluation.Length < 300)
                {
                    theLongth = evaluation.Length;
                }
                evaluation = evaluation.Substring(0, theLongth);
                return Json(evaluation, JsonRequestBehavior.AllowGet);
            }
        }

        //搜索测评分布视图
        public ActionResult SearchEvaluation()
        {
            IEnumerable<tempSearchEvaluation> evaluations = sManager.SearchEvaluation();
            return PartialView(evaluations);
        }

        //搜索测评分布视图
        public ActionResult SearchAnimation()
        {
            IEnumerable<tempSearchAnimation> animations = sManager.SearchAnimation();
            return PartialView(animations);
        }

        //搜索测评分布视图
        public ActionResult SearchUsers()
        {
            IEnumerable<tempSearchUsers> users = sManager.SearchUsers();
            return PartialView(users);
        }
    }
}