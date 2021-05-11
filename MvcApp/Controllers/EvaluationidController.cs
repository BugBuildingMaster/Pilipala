using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BLL;
using Newtonsoft.Json.Linq;

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
        public ActionResult Details(int? id)
        {
            var eva = eManager.GetEvaluation((int)id);
            return View(eva);
        }
        //获取测评内容
        [HttpGet]
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
        public ActionResult AddComment(int aid, int id, string content)
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
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
        }
        //测评点赞
        public ActionResult Addlike(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject name = readtoken(cookie.Values["Token"]);
                return Content(eManager.AddLikeOrDislike(2, id, name["UserName"].ToString()));
            }
        }
        //测评点踩
        public ActionResult AddDislike(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject name = readtoken(cookie.Values["Token"]);
                return Content(eManager.AddLikeOrDislike(3, id, name["UserName"].ToString()));
            }
        }
        //添加测评页
        public ActionResult AddEvaluationid(int? id)
        {
            var e = eManager.GetEvaluation((int)id);
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            ViewBag.id = id;
            ViewBag.Aname = e.Aname;
            ViewBag.username = name["UserName"].ToString();
            return View();
        }
        //添加测评 审核通过后发布
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
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
        public JsonResult DeleteEvaluation(int id)
        {
            bool de = eManager.DeleteEvaluation(id);
            return de ? Json("success") : Json("fail");
        }
    }
}