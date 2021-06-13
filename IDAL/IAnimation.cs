using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;


namespace IDAL
{
    public interface IAnimation
    {
        //获取多个动漫
        IEnumerable<Animation> GetAnimations();
        //根据分类获取动漫
        IEnumerable<Animation> GetAnimations(string name, int num, int currentPage);
        //根据地区获取动漫
        IEnumerable<Animation> GetAnimations(string location,int pages);
        //根据id获取动漫
        Animation GetAnimationById(int id);
        //根据动漫名获取动漫
        Animation GetAnimationByName(string name);
        //根据id获取动漫图片
        string[] GetAimgPicById(int id);
        //获取当前动漫下的评论 时间、热度
        IEnumerable<ShortComment> GetShortCommentsByTime(int aid);
        IEnumerable<ShortComment> GetShortCommentsByHot(int aid);
        //获取动漫简介
        string GetAPlot(int id);
        //添加评论
        bool AddComment(ShortComment sc);
        //评论点赞
        string AddCommentLike(int id, string name, DateTime time);
        //取消评论点赞
        string CancleAddCommentLike(int id, string name);
    }
}
