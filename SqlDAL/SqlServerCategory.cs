using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerCategory : Basedb, ICategory
    {
        /// <summary>
        /// 获取该分类下的动漫数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetCategoryNum(string name)
        {
            var query = db.AnimationCategory.Where(c => c.Categoryname == name).Count();
            return query;
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategory()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var top = db.Category.Where(c => c.Categoryname != "").ToList();
            db.Configuration.ProxyCreationEnabled = true;
            return top;
        }
    }
}
