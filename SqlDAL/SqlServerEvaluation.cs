using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using IDAL;
using SqlDAL;

namespace SqlDAL
{
    public class SqlServerEvaluation : Basedb, IEvaluation
    {

        //根据Id获取测评
        public Evaluation GetEvaluation(int id)
        {
            return db.Evaluation.Find(id);
        }

        //根据动漫名获取当前动漫下的测评
        public IEnumerable<Evaluation> GetEvaluations(string aname)
        {
            return db.Evaluation.Where(e => e.Aname == aname).ToList();
        }

        //根据动漫id获取测评
        public IEnumerable<Evaluation> GetEvaluations(int aid)
        {
            string name = db.Animation.Find(aid).Aname;
            return GetEvaluations(name);
        }

        //获取测评评论
        public IEnumerable<EComment> GetEComments(int eid)
        {
            return db.EComment.Where(e => e.Evaluationid == eid).ToList();
        }

        //添加评论 添加成功返回true
        public bool AddEComment(EComment ect)
        {
            db.EComment.Add(ect);
            return db.SaveChanges() > 0;
        }

        //测评点赞or点踩
        public string AddLikeOrDislike(int num, int id, string username)
        {
            db.LikeOrDislike(num, id, username, DateTime.Now);
            if (num == 2)
            {
                return db.Evaluation.Find(id).Likenum.ToString();
            }
            else if (num == 3)
            {
                return db.Evaluation.Find(id).Dislikenum.ToString();
            }
            return null;
        }
        //添加测评
        public bool AddEvaluation(PublishEvaluation evaluation)
        {
            db.PublishEvaluation.Add(evaluation);
            return db.SaveChanges() > 0;
        }
        //删除测评
        public bool DeleteEvaluation(int eid)
        {
            Evaluation e = db.Evaluation.Find(eid);
            db.Evaluation.Remove(e);
            return db.SaveChanges() > 0;
        }
    }
}
