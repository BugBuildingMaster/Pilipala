using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface IEvaluation
    {
        //获取某篇测评
        Evaluation GetEvaluation(int id);
        //获取某个动漫下的全部测评
        IEnumerable<Evaluation> GetEvaluations(int aid);
        //根据动漫名获取该动漫下的测评
        IEnumerable<Evaluation> GetEvaluations(string aname);
        //根据测评id获取评论
        IEnumerable<EComment> GetEComments(int eid);
        //添加测评评论
        bool AddEComment(EComment ect);
        //测评点赞or点踩
        string AddLikeOrDislike(int num, int id, string username);
        //添加测评 是否添加成功
        bool AddEvaluation(PublishEvaluation evaluation);
        //删除测评
        bool DeleteEvaluation(int eid);
    }
}
