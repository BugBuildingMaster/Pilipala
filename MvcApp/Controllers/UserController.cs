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
    public class UserController : BaseController
    {
        readonly UsersManager uManager = new UsersManager();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        //个人中心视图
        public ActionResult Center(int? id)
        {
            if (Request.Cookies["Login"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject username = readtoken(cookie.Values["Token"]);
                UsersInfo user = uManager.GetUsersInfo((int)id);
                string name = user.UserName;
                var visitor = username["UserName"].ToString();
                if (visitor == user.UserName)
                {
                    ViewBag.username = name;
                    //db.PersonalSpace(name, visitor);
                    return View(user);
                }
                else
                {
                    return RedirectToAction("TouristCenter", "Tourist", new { id, name });
                }
            }
        }



        //修改密码
        public ActionResult EditPassword()
        {
            return PartialView();
        }

        //个人发布测评分布视图
        public ActionResult UserEvaluation()
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            return PartialView(uManager.GetUserEvaluations(name["UserName"].ToString()));
        }

        //短评分布视图
        public ActionResult UserShortComment()
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            return PartialView(uManager.GetUserShortComment(name["UserName"].ToString()));
        }

        //动态分布视图
        public ActionResult UserDynamic()
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            return PartialView(uManager.GetUserDongtai(name["UserName"].ToString()));
        }

        //获取信息
        [HttpGet]
        public JsonResult GetInfo(string name)
        {
            return Json(uManager.GetUsersInfo(name), JsonRequestBehavior.AllowGet);
        }

        // 修改用户头像
        [HttpPost]
        public ActionResult EditPic(FormCollection forms)
        {
            HttpPostedFileBase a = Request.Files["images"];
            if (a != null)
            {
                //获取文件类型
                string fileExtension = Path.GetExtension(a.FileName);
                //自定义文件名（时间+唯一标识符+后缀）
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + Guid.NewGuid() + fileExtension;
                //判断是否存在需要的目录，不存在则创建 
                if (!Directory.Exists(Server.MapPath("~/images/uploads")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/uploads"));
                }
                //拼接保存文件的详细路径
                string savefilePath = Server.MapPath("~/images/uploads/") + fileName;
                a.SaveAs(savefilePath);
                //若扩展名不为空则判断文件是否是指定视频类型
                if (fileExtension != null)
                {
                    if ("(.jpg)|(.png)|(.gif)|(.bmp)".Contains(fileExtension))
                    {
                        //拼接返回的Img标签
                        string dbsrc = "/images/uploads/" + fileName;
                        string username = Session["username"].ToString();
                        bool edit = uManager.EditUsersInfo(username, dbsrc);
                        return edit ? Content(dbsrc) : Content("fail");
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
                else
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("sourcefail");
            }
        }

        //修改个人信息
        public string EditUserInfo(string name, string gender, DateTime? birthday, string signatures)
        {
            bool edit = uManager.EditUserInfos(name, gender, birthday, signatures);
            return edit ? "success" : "fail";
        }


        /*-------------------------------------------------------*/


        readonly RecommendManager rManager = new RecommendManager();

        //历史记录分布视图
        public ActionResult UserWatch(int? page)
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            IEnumerable<Watch> watch = uManager.GetUserHistory(name["UserName"].ToString());
            int PageSize = 10;
            int pageNumber = (page ?? 1);

            return PartialView(watch.ToPagedList(pageNumber, PageSize));
        }

        //动漫分布视图
        public ActionResult UserWatchAnimation(int id)
        {
            return PartialView(uManager.GetUserHistoryAnimation(id));
        }
        //测评分布视图
        public ActionResult UserWatchEvaluation(int id)
        {
            return PartialView(uManager.GetUserHistoryEvaluation(id));
        }

        //添加历史记录
        [HttpGet]
        public ActionResult AddWatch(string type, int id)
        {

            if (Request.Cookies["Login"] == null)
            {
                return Json("login", JsonRequestBehavior.AllowGet);
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject name = readtoken(cookie.Values["Token"]);
                rManager.AddWatch(type, id, name["UserName"].ToString());
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
        }

        //删除历史记录
        [HttpPost]
        public JsonResult DeleteWatch(int id)
        {
            bool flag = rManager.DeleteWatch(id);
            if (flag)
            {
                return Json("Success");
            }
            else
            {
                return Json("Fail");
            }
        }

        //关注列表
        [HttpGet]
        public JsonResult UserFollow(int page)
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            IEnumerable<tempFollow> follows = uManager.GetUserFollows(page, name["UserName"].ToString(), name["UserName"].ToString());
            return Json(follows, JsonRequestBehavior.AllowGet);
        }

        //粉丝列表
        [HttpGet]
        public JsonResult UserFans(int page)
        {
            HttpCookie cookie = Request.Cookies["Login"];
            JObject name = readtoken(cookie.Values["Token"]);
            IEnumerable<tempFans> fans = uManager.GetUserFans(page, name["UserName"].ToString(), name["UserName"].ToString());
            return Json(fans, JsonRequestBehavior.AllowGet);
        }

        //关注用户或取消关注用户
        [HttpGet]
        public ActionResult Following(int id)
        {
            if (Request.Cookies["Login"] == null)
            {
                return Content("login");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Login"];
                JObject name = readtoken(cookie.Values["Token"]);
                bool falg = uManager.Following(id, name["UserName"].ToString());
                if (falg)
                {
                    return Content("Success");
                }
                else
                {
                    return Content("fail");
                }
            }
        }
    }
}