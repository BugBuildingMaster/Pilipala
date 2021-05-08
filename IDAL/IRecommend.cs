using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface IRecommend
    {
        bool AddWatch(string type, int id,string name);     //添加浏览记录
        IEnumerable<tempRecommendAnimation> GetRecommendAnimation(string name);     //获取推荐动漫
        bool DeleteWatch(int id);       //删除浏览记录
    }
}
