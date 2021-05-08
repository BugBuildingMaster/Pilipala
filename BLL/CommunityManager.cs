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
    public class CommunityManager
    {
        readonly ICommunity icommunity = DataAccess.CreateCommunity();

        #region 动态获取
        public bool Dynamic(string name)
        {
            return icommunity.Dynamic(name);
        }
        #endregion

        #region 动态详情获取
        public dongtai DynamicDetail(int id)
        {
            return icommunity.DynamicDetail(id);
        }
        #endregion

        #region 动态分布视图
        public IEnumerable<tempCommunityDongtai> AllDynamic()
        {
            return icommunity.AllDynamic();
        }
        #endregion

        #region 动态评论分布视图
        public IEnumerable<dongtaiComment> DynamicComment(int id)
        {
            return icommunity.DynamicComment(id);
        }
        #endregion

        #region 动态回复分布视图
        public IEnumerable<dongtaiCommentReply> DynamicReply(int id)
        {
            return icommunity.DynamicReply(id);
        }
        #endregion


        #region 测评分布视图
        public IEnumerable<tempCommunityEvaluation> AllEvaluation()
        {
            return icommunity.AllEvaluation();
        }
        #endregion

        #region 短评分布视图
        public IEnumerable<tempCommunityShortComment> AllShortComment()
        {
            return icommunity.AllShortComment();
        }
        #endregion

        #region 返回指定用户头像地址

        public string GetPortrait(string name)
        {
            return icommunity.GetPortrait(name);
        }
        #endregion

        #region 返回指定用户id
        public string GetID(string name)
        {
            return icommunity.GetID(name);
        }
        #endregion


        #region 添加动态
        public bool AddDynamic(string content,string name)
        {
            return icommunity.AddDynamic(content,name);
        }
        #endregion

        #region 添加评论
        public bool AddComment(int dtid, string content,string name)    //参数为动态id，评论文本，评论者名
        {
            return icommunity.AddComment(dtid,content, name);
        }
        #endregion

        #region 动态评论回复添加
        public bool AddCommentReply(int id, string content, int dtid,string name)
        {
            return icommunity.AddCommentReply(id, content,dtid, name);
        }
        #endregion

        #region 动态删除
        public bool DeleteDynamic(int id)
        {
            bool flag = icommunity.DeleteDynamic(id);
            return flag;
        }
        #endregion

        #region 删除评论
        public bool DeleteComment(int dtid)
        {
            bool flag = icommunity.DeleteComment(dtid);
            return flag;
        }
        #endregion

        #region 删除评论回复
        public bool DeleteReply(int dtid)
        {
            bool flag = icommunity.DeleteReply(dtid);
            return flag;
        }
        #endregion

        #region 动态点赞

        public string AddLike(int id, string name)
        {
            string num = icommunity.AddLike(id,name);
            return num;
        }
        #endregion

        #region 短评点赞
        public string ShortCommentAddLike(int id, string name)
        {
            string num = icommunity.ShortCommentAddLike (id,name);
            return num;
        }
        #endregion

        #region 动态评论点赞
        public string DongtaiCommentAddLike(int id, string name)
        {
            string num = icommunity.DongtaiCommentAddLike(id,name);
            return num;
        }
        #endregion

        #region 动态评论数返回
        public string ShortCommentNum(int id)
        {
            string num = icommunity.ShortCommentNum(id);
            return num;
        }
        #endregion
    }
}
