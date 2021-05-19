using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;
using PagedList;
using MvcThrottle;

namespace MvcApp.Controllers
{
    public class RankingListController : Controller
    {
        readonly RankinglistManager rManager = new RankinglistManager();

        // GET: RankingList

        [HttpGet]
        public JsonResult TheRankingList(int? type, int page)
        {
            type = type ?? 1;
            //调用存储过程更新视图数据
            if (type == 1)
            {
                rManager.UpdateRankingList("动画", 1);
            }
            else
            {
                string name = rManager.GetPartialRank((int)type);
                rManager.UpdateRankingList(name, 1);
            }
            IEnumerable<tempRankingList> Animations = rManager.GetRankingLists(page);

            return Json(Animations, JsonRequestBehavior.AllowGet);
        }

        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Rankinglist()
        {
            return View();
        }

        public JsonResult test(int? type)
        {
            string name = rManager.GetPartialRank((int)type);
            rManager.UpdateRankingList(name, 1);
            IEnumerable<tempRankingList> Animations = rManager.GetRankingLists(1);

            return Json(Animations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RankingCategory()
        {
            IEnumerable<Category> cate = rManager.RankingCategory();
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        //返回分布视图
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult GetPartialRank(int id, int page)
        {
            string name = rManager.GetPartialRank(id);
            rManager.UpdateRankingList(name, 1);

            IEnumerable<tempRankingList> Animations = rManager.GetRankingLists(page);
            int PageSize = 10;
            int pageNumber = 1;

            return PartialView("PartialRankingList", Animations.ToPagedList(pageNumber, PageSize));
        }
    }
}