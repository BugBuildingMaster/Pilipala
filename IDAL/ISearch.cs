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
        //搜索
        bool Search(string KeyWord,string user);
        //搜索测评分布视图
        IEnumerable<tempSearchEvaluation> SearchEvaluation();
        //搜索动漫分布视图
        IEnumerable<tempSearchAnimation> SearchAnimation();
        //搜索用户分布视图
        IEnumerable<tempSearchUsers> SearchUsers();
        //测评预览
        string GetEvaluationPreview(int id);
    }
}
