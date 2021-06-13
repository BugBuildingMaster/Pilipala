using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using SqlDAL;
using IDAL;
using Factory;

namespace BLL
{
    public class EvaluationManager
    {
        readonly IEvaluation evaluation = DataAccess.CreateEvaluation();
        readonly ICommunity community = DataAccess.CreateCommunity();
        /// <summary>
        /// 获取某一篇测评
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Evaluation GetEvaluation(int id)
        {
            return evaluation.GetEvaluation(id);
        }
        /// <summary>
        /// 根据动漫名获取该动漫下的测评
        /// </summary>
        /// <param name="aname"></param>
        /// <returns></returns>
        public IEnumerable<Evaluation> GetEvaluations(string aname)
        {
            return evaluation.GetEvaluations(aname);
        }
        /// <summary>
        /// 获取测评评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<EComment> GetEComments(int id)
        {
            return evaluation.GetEComments(id);
        }
        /// <summary>
        /// 判断是否添加评论
        /// </summary>
        /// <param name="ect"></param>
        /// <returns></returns>
        public bool AddComment(EComment ect)
        {
            return evaluation.AddEComment(ect);
        }
        /// <summary>
        /// 测评点赞or点踩
        /// </summary>
        /// <param name="num"></param>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string AddLikeOrDislike(int num, int id, string username, DateTime time)
        {
            if (num == 2)
            {
                if (community.LikeExist(id, username, "EvaluationLike"))
                    return evaluation.CancleAddLike(id, username);
                else
                    return evaluation.AddLike(id, username,time);
            }
            else
            {
                if (community.LikeExist(id, username, "EvaluationDisLike"))
                    return evaluation.CancleAddDislike(id, username);
                else
                    return evaluation.AddDislike(id, username,time);
            }


        }
        /// <summary>
        /// 发布测评 先审核
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool AddEvaluation(PublishEvaluation e)
        {
            return evaluation.AddEvaluation(e);
        }
        /// <summary>
        /// 删除测评
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteEvaluation(int id)
        {
            return evaluation.DeleteEvaluation(id);
        }
        public IEnumerable<Evaluation> GetEvaluations(int aid)
        {
            return evaluation.GetEvaluations(aid);
        }
    }
}

