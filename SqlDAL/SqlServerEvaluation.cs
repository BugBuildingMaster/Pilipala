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

        //测评点赞
        public string AddLike(int id, string username, DateTime time)
        {
            Elike like = new Elike
            {
                UserName = username,
                Evaluationid = id,
                Time = time
            };
            db.Elike.Add(like);
            db.SaveChanges();
            return db.Evaluation.Find(id).Likenum.ToString();
        }
        //取消测评点赞
        public string CancleAddLike(int id, string username)
        {
            Elike like = db.Elike.Where(x => x.UserName == username && x.Evaluationid == id).FirstOrDefault();
            db.Elike.Remove(like);
            db.SaveChanges();
            return db.Evaluation.Find(id).Likenum.ToString();
        }
        //测评点踩
        public string AddDislike(int id, string username, DateTime time)
        {
            Edislike like = new Edislike
            {
                UserName = username,
                Evaluationid = id,
                Time = time
            };
            db.Edislike.Add(like);
            db.SaveChanges();
            return db.Evaluation.Find(id).Dislikenum.ToString();
        }
        //取消测评点踩
        public string CancleAddDislike(int id, string username)
        {
            Edislike like = db.Edislike.Where(x => x.UserName == username && x.Evaluationid == id).FirstOrDefault();
            db.Edislike.Remove(like);
            db.SaveChanges();
            return db.Evaluation.Find(id).Dislikenum.ToString();
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
