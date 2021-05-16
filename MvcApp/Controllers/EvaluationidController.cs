using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BLL;
using Newtonsoft.Json.Linq;
using MvcThrottle;

namespace MvcApp.Controllers
{
    public class EvaluationidController : BaseController
    {
        readonly EvaluationManager eManager = new EvaluationManager();
        readonly AnimationManager aManager = new AnimationManager();

        // GET: Evaluationid
        public ActionResult Index()
        {
            return View();
        }

        //返回一个测评详情页
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Details(int? id)
        {
            var eva = eManager.GetEvaluation((int)id);
            return View(eva);
        }
        //获取测评内容
        [HttpGet]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public JsonResult GetEvaluation(int id)
        {
            var evaluation = eManager.GetEvaluation(id).Content;
            return Json(evaluation, JsonRequestBehavior.AllowGet);
        }
        //测评评论分布视图
        public ActionResult EComment(int id)
        {
            var ec = eManager.GetEComments(id);
            return PartialView(ec);
        }
        //添加评论
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult AddComment(int aid, int id, string content)
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
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
                    EComment etc = new EComment
                    {
                        UserName = name["UserName"].ToString(),
                        Animationid = aid,
                        Evaluationid = id,
                        Time = DateTime.Now,
                        Likenum = 0,
                        content = content
                    };
                    bool add = eManager.AddComment(etc);
                    if (add)
                    {
                        var comm = eManager.GetEComments(id);
                        return PartialView("EComment", comm);
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
                else
                {
                    return Content("login");
                }
            }
        }
        //测评点赞
        [EnableThrottling(PerSecond = 7, PerMinute = 400, PerHour = 3000, PerDay = 10000)]
        public ActionResult Addlike(int id)
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
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
                    return Content(eManager.AddLikeOrDislike(2, id, name["UserName"].ToString()));
                }
                else
                {
                    return Content("login");
                }
            }
        }
        //测评点踩
        [EnableThrottling(PerSecond = 7, PerMinute = 400, PerHour = 3000, PerDay = 10000)]
        public ActionResult AddDislike(int id)
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
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
                    return Content(eManager.AddLikeOrDislike(3, id, name["UserName"].ToString()));
                }
                else
                {
                    return Content("login");
                }
            }
        }
        //添加测评页
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult AddEvaluationid(int? id)
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
                string url = Request.Url.ToString();
                System.Web.HttpContext.Current.Session["thePass"] = url;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                var e = eManager.GetEvaluation((int)id);
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    ViewBag.id = id;
                    ViewBag.Aname = e.Aname;
                    ViewBag.username = name["UserName"].ToString();
                    return View();
                }
                else
                {
                    string url = Request.Url.ToString();
                    System.Web.HttpContext.Current.Session["thePass"] = url;
                    return RedirectToAction("Login", "Users");
                }
            }

        }
        //添加测评 审核通过后发布
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [EnableThrottling(PerSecond = 4, PerMinute = 40, PerHour = 300, PerDay = 400)]
        public ActionResult AddEvaluation(int id, string name, Evaluation evaluation)
        {
            //新建待发布测评对象
            PublishEvaluation pe = new PublishEvaluation
            {
                Aname = aManager.GetAnimation(id).Aname,
                Content = evaluation.Content,
                Publishname = name,
                Score = evaluation.Score,
                Time = DateTime.Now,
                Result = "待处理",
                Title = evaluation.Title
            };
            //添加到待发布表中
            bool add = eManager.AddEvaluation(pe);
            if (add)
            {
                return Content("success");
            }
            else
            {
                return Content("fail");
            }

        }
        //删除测评
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public JsonResult DeleteEvaluation(int id)
        {
            bool de = eManager.DeleteEvaluation(id);
            return de ? Json("success") : Json("fail");
        }
    }
}