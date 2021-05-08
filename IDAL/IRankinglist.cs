using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface IRankinglist
    {
        //根据获取的分类名更新排行榜视图
        bool UpdateRankingList(string name, int id);
        //获取排行榜分类
        IEnumerable<Category> RankingCategory();
        //根据id获取分类名
        string GetPartialRank(int id);
        //返回排行榜内容
        IEnumerable<tempRankingList> GetRankingLists(int page);
    }
}
