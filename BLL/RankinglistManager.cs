using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using IDAL;
using Factory;

namespace BLL
{
    public class RankinglistManager
    {
        readonly IRankinglist irankinglist = DataAccess.CreateRankinglist();


        #region 根据获取的分类名更新排行榜视图
        public bool UpdateRankingList(string name, int id)
        {
            return irankinglist.UpdateRankingList(name, id);
        }
        #endregion

        #region 获取排行榜分类
        public IEnumerable<Category> RankingCategory()
        {
            return irankinglist.RankingCategory();

        }
        #endregion

        #region 根据id获取分类名
        public string GetPartialRank(int id)
        {
            return irankinglist.GetPartialRank(id);

        }
        #endregion

        #region 返回排行榜内容
        public IEnumerable<tempRankingList> GetRankingLists(int page)
        {
            return irankinglist.GetRankingLists(page);

        }
        #endregion
    }
}
