using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface ISearch
    {
        bool Search(string KeyWord,string user);   //搜索
        IEnumerable<tempSearchUsers> SearchUsers();   //搜索用户分布视图
        string GetEvaluationPreview(int id);   //测评预览
    }
}
