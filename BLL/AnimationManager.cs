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
    public class AnimationManager
    {
        readonly IAnimation animation = DataAccess.CreateAnimations();
        readonly ICommunity icommunity = DataAccess.CreateCommunity();

        #region 根据个数获取日本动漫
        public IEnumerable<Animation> GetJanpanAniamtion(int pages)
        {
            return animation.GetAnimations("日本", pages);
        }
        #endregion

        #region 根据个数返回中国动漫
        public IEnumerable<Animation> GetChinaAnimation(int pages)
        {
            return animation.GetAnimations("中国大陆", pages);
        }
        #endregion

        #region 分页获取动漫
        public IEnumerable<Animation> GetAnimationByPages(string name, int num, int currentPage)
        {
            return animation.GetAnimations(name, num, currentPage);
        }
        #endregion

        #region 获取动漫（动漫名、id）
        public Animation GetAnimation(int id)
        {
            return animation.GetAnimationById(id);
        }

        public Animation GetAnimation(string name)
        {
            return animation.GetAnimationByName(name);
        }
        #endregion

        #region 获取当前动漫照片墙
        public string[] GetAnimationPhotos(int aid)
        {
            return animation.GetAimgPicById(aid);
        }

        #endregion

        /// <summary>
        /// 获取短评 control为1时按时间排序 否则按热度排序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public IEnumerable<ShortComment> GetShortComments(int id, int control)
        {
            return control == 1 ? animation.GetShortCommentsByTime(id) : animation.GetShortCommentsByHot(id);
        }

        //获取简介
        public string GetApolt(int id)
        {
            return animation.GetAPlot(id);
        }
        /// <summary>
        /// 添加动漫短评
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public bool AddComment(ShortComment sc)
        {
            return animation.AddComment(sc);
        }

        public string AddCommentLike(int id, string name, DateTime time)
        {
            if (icommunity.LikeExist(id, name, "ShortComment"))
                return animation.CancleAddCommentLike(id, name);
            else
                return animation.AddCommentLike(id, name,time);

        }
    }
}
