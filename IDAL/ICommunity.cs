using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface ICommunity
    {
        bool Dynamic(string name);       //社区初始化
        dongtai DynamicDetail(int id);                   //动态详情获取
        IEnumerable<tempCommunityDongtai> AllDynamic();  //动态分布视图
        IEnumerable<dongtaiComment> DynamicComment(int id);    //动态评论分布视图
        IEnumerable<dongtaiCommentReply> DynamicReply(int id); //动态回复分布视图

        IEnumerable<tempCommunityEvaluation> AllEvaluation(); //测评分布视图
        IEnumerable<tempCommunityShortComment> AllShortComment(); //短评分布视图

        string GetPortrait(string name);    //返回指定用户头像地址
        string GetID(string name);          //返回指定用户id

        bool AddDynamic(string content,string name);                 //添加动态
        bool AddComment(int dtid, string content,string name);//添加评论
        bool AddCommentReply(int id, string content, int dtid,string name);   //动态评论回复添加
        bool DeleteDynamic(int id);         //动态删除
        bool DeleteComment(int dtid);       //删除评论
        bool DeleteReply(int dtid);         //删除评论回复

        string AddLike(int id,string name);                 //动态点赞
        string ShortCommentAddLike(int id, string name);     //短评点赞
        string DongtaiCommentAddLike(int id, string name);   //动态评论点赞
        string ShortCommentNum(int id);         //动态评论数返回
    }
}
