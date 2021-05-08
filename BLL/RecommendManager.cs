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
    public class RecommendManager
    {
        readonly IRecommend irecommend = DataAccess.CreateRecommend();

        #region 添加用户的浏览记录
        public bool AddWatch(string type, int id, string name)
        {
            return irecommend.AddWatch(type, id, name);
        }
        #endregion

        #region 获取用户的推荐动漫
        public IEnumerable<tempRecommendAnimation> GetRecommendAnimation(string name)
        {
            return irecommend.GetRecommendAnimation(name);
        }
        #endregion

        #region 删除用户的浏览记录
        public bool DeleteWatch(int id)
        {
            return irecommend.DeleteWatch(id);
        }
        #endregion
    }
}
