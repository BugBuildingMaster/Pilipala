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
                UserName =name,
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
        public string AddLike(int id, string name)
        {
            db.LikeOrDislike(4, id, name, DateTime.Now);
            var i = db.dongtai.Find(id).Likenum.ToString();
            return i;
        }
        #endregion

        #region 短评点赞
        public string ShortCommentAddLike(int id, string name)
        {
            db.LikeOrDislike(1, id, name, DateTime.Now);
            var i = db.ShortComment.Find(id).Likenum.ToString();
            return i;
        }
        #endregion

        #region 动态评论点赞
        public string DongtaiCommentAddLike(int id, string name)
        {
            db.LikeOrDislike(5, id, name, DateTime.Now);
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
    }
}
