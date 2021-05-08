using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using IDAL;
using SqlDAL;
using Factory;

namespace BLL
{    
    public class CategoryManager
    {
        readonly ICategory category = DataAccess.CreateCategory();
        /// <summary>
        /// 根据分类名获取该分类下的动漫数量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetCategoryNums(string name)
        {
            return category.GetCategoryNum(name);
        }

        /// <summary>
        /// 返回所有的类别名
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategories()
        {
            return category.GetCategory();
        }
    }
}
