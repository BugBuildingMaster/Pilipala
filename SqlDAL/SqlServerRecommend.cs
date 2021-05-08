using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerRecommend : Basedb, IRecommend
    {
        //添加历史记录
        public bool AddWatch(string type, int id, string name)
        {
            db.WatchUpdate(id, type, name, DateTime.Now);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteWatch(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Watch w = db.Watch.Find(id);
            db.Watch.Remove(w);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //获取用户的推荐动漫
        public IEnumerable<tempRecommendAnimation> GetRecommendAnimation(string name)
        {
            db.RecommendSet(name);
            return db.tempRecommendAnimation;
        }
    }
}
