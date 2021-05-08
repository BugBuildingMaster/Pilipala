using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerRankinglist : Basedb, IRankinglist
    {
        //根据获取的分类名更新排行榜视图
        public bool UpdateRankingList(string name, int id)
        {
            db.RankingListSet(name, id);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //获取排行榜分类
        public IEnumerable<Category> RankingCategory()
        {
            return db.Category.Where(c => c.Categoryname != null).ToList();
        }

        //根据id获取分类名
        public string GetPartialRank(int id)
        {
            return db.Category.Find(id).Categoryname.ToString();
        }

        //返回排行榜内容
        public IEnumerable<tempRankingList> GetRankingLists(int page)
        {
            return db.tempRankingList.OrderByDescending(o => o.Score).Take(page * 20).Skip((page - 1) * 20).ToList();
        }
    }
}
