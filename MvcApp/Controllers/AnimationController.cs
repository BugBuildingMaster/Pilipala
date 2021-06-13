using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using Models;
using Newtonsoft.Json.Linq;
using MvcThrottle;

namespace MvcApp.Controllers
{
    public class AnimationController : BaseController
    {
        readonly AnimationManager aManager = new AnimationManager();
        readonly CategoryManager cManager = new CategoryManager();
        readonly EvaluationManager emanager = new EvaluationManager();
        /// <summary>
        /// 项目首页
        /// </summary>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Index()
        {
            try
            {
                InitUpdater();
                if (Request.Cookies["Login"] != null && Request.Cookies["Key"] != null)
                {
                    HttpCookie cookie = Request.Cookies["Login"];
                    string tokenContent = cookie.Values["Token"];
                    string pubKey = Request.Cookies["Key"].Value;
                    if (VerToken(tokenContent, pubKey))
                    {
                        JObject name = readtoken(cookie.Values["Token"]);
                        ViewBag.username = name["UserName"].ToString();
                        ViewBag.userid = name["UserId"].ToString();
                    }
                    else
                    {
                        ViewBag.username = null;
                        ViewBag.userid = null;
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.username = null;
                ViewBag.userid = null;
                return View();
            }
            return View();
        }

        /// <summary>
        /// 动漫详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            //ViewBag.id = id;
            //return View(aManager.GetAnimation((int)id));
            InitUpdater();
            if (id == null)
            {
                return RedirectToAction("Index", "Animation");
            }
            else
            {
                AddWatch("Animation", (int)id);
                ViewBag.id = id;
                return View(aManager.GetAnimation((int)id));
            }
        }

        // 获取日本动漫
        [HttpGet]
        public JsonResult GetJapanAniamtions(int pages)
        {
            return Json(aManager.GetJanpanAniamtion(pages), JsonRequestBehavior.AllowGet);
        }
        // 获取中国大陆动漫（不含中国香港、中国台湾、中国澳门地区）
        [HttpGet]
        public JsonResult GetChinaAniamtions(int pages)
        {
            return Json(aManager.GetChinaAnimation(pages), JsonRequestBehavior.AllowGet);
        }
        // 返回动漫分类
        [HttpGet]
        public JsonResult GetCategories()
        {
            return Json(cManager.GetCategories(), JsonRequestBehavior.AllowGet);
        }
        // 分页获取动漫信息
        [HttpGet]
        public JsonResult GetaByCategory(string name, int num, int currentPage)
        {
            return Json(aManager.GetAnimationByPages(name, num, currentPage), JsonRequestBehavior.AllowGet);
        }
        // 获取分页下的动漫数
        public JsonResult GetaCategoryNum(string name)
        {
            return Json(cManager.GetCategoryNums(name), JsonRequestBehavior.AllowGet);
        }
        //获取动漫下的短评
        public ActionResult ShortComment(int id)
        {
            return PartialView(aManager.GetShortComments(id, 1));
        }
        //获取动漫的照片墙
        public JsonResult Photoes(int id)
        {
            return Json(aManager.GetAnimationPhotos(id), JsonRequestBehavior.AllowGet);
        }
        //获取动漫简介
        public string GetPlot(int id)
        {
            return aManager.GetApolt(id);
        }
        //当前动漫下的测评分布视图
        public ActionResult Evaluation(int id)
        {
            var es = emanager.GetEvaluations(id);
            return PartialView(es);
        }
        //添加评论
        public ActionResult AddComment(int id, string content, int score)
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
                    ShortComment sc = new ShortComment
                    {
                        UserName = name["UserName"].ToString(),
                        Animationid = id,
                        Time = DateTime.Now,
                        content = content,
                        Likenum = 0,
                        Score = score
                    };
                    bool add = aManager.AddComment(sc);
                    if (add)
                    {
                        var comm = aManager.GetShortComments(id, 1);
                        return PartialView("ShortComment", comm);
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
        //判断用户是否登录
        [HttpGet]
        public string isUserLogin()
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
                return "login";
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    return "success";
                }
                else
                {
                    return "login";
                }
            }
        }
        //短评点赞
        [HttpPost]
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
                    //return Content(aManager.AddCommentLike(id, name["UserName"].ToString(), DateTime.Now));
                    return Content(ToLike(id, name["UserName"].ToString(), "ShortComment", DateTime.Now, false));
                }
                else
                {
                    return Content("login");
                }
            }
        }

        /*----------------------------------------------------------------------------------*/

        readonly RecommendManager rmanager = new RecommendManager();

        //推荐分布视图
        public ActionResult Recommend()
        {
            if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)
            {
                return null;
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                string tokenContent = cookie.Values["Token"];
                string pubKey = Request.Cookies["Key"].Value;
                if (VerToken(tokenContent, pubKey))
                {
                    JObject name = readtoken(cookie.Values["Token"]);
                    return PartialView(rmanager.GetRecommendAnimation(name["UserName"].ToString()));
                }
                else
                {
                    return null;
                }
            }
        }

    }
}