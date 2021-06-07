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
    public class SearchManager
    {
        readonly ISearch isearch = DataAccess.CreateSearch();

        #region 搜索
        public bool Search(string KeyWord,string user)
        {
            bool flag = isearch.Search(KeyWord,user);
            return flag;
        }
        #endregion

        #region 搜索用户分布视图
        public IEnumerable<tempSearchUsers> SearchUsers()
        {
            return isearch.SearchUsers();
        }
        #endregion

        #region 测评预览
        public string GetEvaluationPreview(int id)
        {
            return isearch.GetEvaluationPreview(id);
        }
        #endregion
    }
}
