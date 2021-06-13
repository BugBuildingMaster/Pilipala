using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerCommunity : Basedb, ICommunity
    {
        #region 动态获取
        public bool Dynamic(string name)
        {
            db.CommunitySet(name);
            return true;
        }
        #endregion

        #region 动态详情获取
        public dongtai DynamicDetail(int id)
        {
            return db.dongtai.Find(id);
        }
        #endregion

        #region 动态分布视图
        public IEnumerable<tempCommunityDongtai> AllDynamic()
        {
            return db.tempCommunityDongtai.OrderByDescending(c => c.Time);
        }
        #endregion

        #region 动态评论分布视图
        public IEnumerable<dongtaiComment> DynamicComment(int id)
        {
            return db.dongtaiComment.Where(c => c.dongtaiid == id).OrderByDescending(c => c.Time);
        }
        #endregion

        #region 动态回复分布视图
        public IEnumerable<dongtaiCommentReply> DynamicReply(int id)
        {
            return db.dongtaiCommentReply.Where(c => c.dtCommentid == id).OrderByDescending(c => c.Time);
        }
        #endregion


        #region 测评分布视图
        public IEnumerable<tempCommunityEvaluation> AllEvaluation()
        {
            return db.tempCommunityEvaluation.OrderByDescending(c => c.Time);
        }
        #endregion

        #region 短评分布视图
        public IEnumerable<tempCommunityShortComment> AllShortComment()
        {
            return db.tempCommunityShortComment.OrderByDescending(c => c.Time);
        }
        #endregion


        #region 返回指定用户头像地址
        public string GetPortrait(string name)
        {
            return db.UsersInfo.Find(name).Portrait.ToString();
        }
        #endregion

        #region 返回指定用户id
        public string GetID(string name)
        {
            return db.Users.Find(name).Userid.ToString();
        }
        #endregion


        #region 添加动态
        public bool AddDynamic(string content, string name)
        {
            dongtai dt = new dongtai
            {
                Content = content,
                UserName = name,
                Likenum = 0,
                Commentnum = 0,
                Time = DateTime.Now
            };
            db.dongtai.Add(dt);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 添加评论
        public bool AddComment(int dtid, string content, string name)
        {
            dongtaiComment dtc = new dongtaiComment
            {
                UserName = name,
                dongtaiid = dtid,
                Time = DateTime.Now,
                Likenum = 0,
                Content = content
            };
            db.dongtaiComment.Add(dtc);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 动态评论回复添加
        public bool AddCommentReply(int id, string content, int dtid, string name)
        {
            dongtaiCommentReply dtc = new dongtaiCommentReply
            {
                UserName = name,
                dtCommentid = id,
                Time = DateTime.Now,
                Content = content
            };
            db.dongtaiCommentReply.Add(dtc);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 动态删除
        public bool DeleteDynamic(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            dongtai e = db.dongtai.Find(id);
            db.dongtai.Remove(e);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 删除评论
        public bool DeleteComment(int dtid)
        {
            dongtaiComment e = db.dongtaiComment.Find(dtid);
            db.dongtaiComment.Remove(e);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 删除评论回复
        public bool DeleteReply(int dtid)
        {
            dongtaiCommentReply e = db.dongtaiCommentReply.Find(dtid);

            db.dongtaiCommentReply.Remove(e);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region 动态点赞
        public string AddLike(int id, string name, DateTime time)
        {
            dongtailike like = new dongtailike
            {
                UserName = name,
                dongtaiid = id,
                Time = time
            };
            db.dongtailike.Add(like);
            db.SaveChanges();
            var i = db.dongtai.Find(id).Likenum.ToString();
            return i;
        }
        public string CancleAddLike(int id, string name)
        {
            dongtailike like = db.dongtailike.Where(x => x.UserName == name && x.dongtaiid == id).FirstOrDefault();
            db.dongtailike.Remove(like);
            db.SaveChanges();
            var i = db.dongtai.Find(id).Likenum.ToString();
            return i;
        }
        #endregion

        #region 短评点赞
        public string ShortCommentAddLike(int id, string name, DateTime time)
        {
            SLike like = new SLike
            {
                SuserName = name,
                Scommentid = id,
                Time = time
            };
            db.SLike.Add(like);
            db.SaveChanges();
            var i = db.ShortComment.Find(id).Likenum.ToString();
            return i;
        }
        public string CancleShortCommentAddLike(int id, string name)
        {
            SLike like = db.SLike.Where(x => x.SuserName == name && x.Scommentid == id).FirstOrDefault();
            db.SLike.Remove(like);
            db.SaveChanges();
            var i = db.ShortComment.Find(id).Likenum.ToString();
            return i;
        }
        #endregion

        #region 动态评论点赞
        public string DongtaiCommentAddLike(int id, string name,DateTime time)
        {
            dongtaiCommentlike like = new dongtaiCommentlike
            {
                UserName = name,
                dongtaiCommentid = id,
                Time = time
            };
            db.dongtaiCommentlike.Add(like);
            db.SaveChanges();
            var i = db.dongtaiComment.Find(id).Likenum.ToString();
            return i;
        }
        public string CancleDongtaiCommentAddLike(int id, string name)
        {
            dongtaiCommentlike like = db.dongtaiCommentlike.Where(x => x.UserName == name && x.dongtaiCommentid == id).FirstOrDefault();
            db.dongtaiCommentlike.Remove(like);
            db.SaveChanges();
            var i = db.dongtaiComment.Find(id).Likenum.ToString();
            return i;
        }
        #endregion

        #region 动态评论数返回
        public string ShortCommentNum(int id)
        {
            var i = db.dongtai.Find(id).Commentnum.ToString();
            return i;
        }
        #endregion

        #region 判断是否有点赞记录
        public bool LikeExist(int id, string name, string type)
        {
            bool exists;
            switch (type)
            {
                case "ShortComment":
                    exists = db.SLike.Any(x => x.Scommentid == id && x.SuserName == name);
                    return exists;
                case "Dongtai":
                    exists = db.dongtailike.Any(x => x.dongtaiid == id && x.UserName == name);
                    return exists;
                case "DongtaiComment":
                    exists = db.dongtaiCommentlike.Any(x => x.dongtaiCommentid == id && x.UserName == name);
                    return exists;
                case "EvaluationLike":
                    exists = db.Elike.Any(x => x.Evaluationid == id && x.UserName == name);
                    return exists;
                case "EvaluationDisLike":
                    exists = db.Edislike.Any(x => x.Evaluationid == id && x.UserName == name);
                    return exists;
                case "Message":
                    exists = db.Mlike.Any(x => x.Messageid == id && x.UserName == name);
                    return exists;
                case "EvaluationCoLike":
                    exists = db.EClike.Any(x => x.ECcommentid == id && x.UserName == name);
                    return exists;
                case "BuzzwordLike":
                    exists = db.BWlike.Any(x => x.Buzzwordid == id && x.UserName == name);
                    return exists;
                case "BuzzwordDisLike":
                    exists = db.BWdislike.Any(x => x.Buzzwordid == id && x.UserName == name);
                    return exists;
                default:
                    return false;
            }
        }
        #endregion

        #region 获取点赞数
        public string GetNumber(int id,string type)
        {
            switch (type)
            {
                case "ShortComment":
                    return db.ShortComment.Find(id).Likenum.ToString();
                case "Dongtai":
                    return db.dongtai.Find(id).Likenum.ToString();
                case "DongtaiComment":
                    return db.dongtaiComment.Find(id).Likenum.ToString();
                case "EvaluationLike":
                    return db.Evaluation.Find(id).Likenum.ToString();
                case "EvaluationDisLike":
                    return db.Evaluation.Find(id).Dislikenum.ToString();
                case "Message":
                    return db.Message.Find(id).Likenum.ToString();
                case "EvaluationCoLike":
                    return db.EComment.Find(id).Likenum.ToString();
                case "BuzzwordLike":
                    return db.Buzzword.Find(id).Likenum.ToString();
                case "BuzzwordDisLike":
                    return db.Buzzword.Find(id).Dislike.ToString();
                default:
                    return null;
            }
        }
        #endregion
    }
}
