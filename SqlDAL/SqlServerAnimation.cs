using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerAnimation : Basedb,IAnimation
    {
        #region 根据Id获取动漫图片
        public string[] GetAimgPicById(int id)
        {
            var ap = db.AnimationPhoto.Where(p => p.Animationid == id).Select(a => a.Alink).ToArray();
            db.Configuration.ProxyCreationEnabled = true;
            return ap;
        }
        #endregion

        #region 获取动漫
        /// <summary>
        /// 根据id获取动漫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Animation GetAnimationById(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var a = (Animation)db.Animation.Find(id);
            db.Configuration.ProxyCreationEnabled = true;
            return a;
        }
        /// <summary>
        /// 根据名称返回动漫
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Animation GetAnimationByName(string name)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var a = db.Animation.Where(c => c.Aname == name).FirstOrDefault();
            db.Configuration.ProxyCreationEnabled = true;
            return a;
        }
        #endregion

        #region 根据分类名、数量、页号返回动漫
        public IEnumerable<Animation> GetAnimations(string name, int num, int currentPage)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //根据分类获取到相应的动漫信息
            IEnumerable<Animation> query = from a in db.Animation
                                           join c in db.AnimationCategory
                                           on a.Animationid equals c.Animationid
                                           where c.Categoryname == name
                                           orderby a.Animationid
                                           select a;
            //根据数值取出条数
            return query.Skip(currentPage * num).Take(num);
        }
        #endregion

        #region 根据地区、个数返回动漫
        public IEnumerable<Animation> GetAnimations(string location, int pages)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Animation.Where(a => a.Alocation == location).Take(pages).ToList();
        }

        #endregion

        #region 获取当前动漫的短评
        public IEnumerable<ShortComment> GetShortCommentsByTime(int aid)
        {
            var comm = db.ShortComment.Include("Users").Where(c => c.Animationid == aid).OrderByDescending(c => c.Time).ToList();
            return comm;
        }
        public IEnumerable<ShortComment> GetShortCommentsByHot(int aid)
        {
            var comm = db.ShortComment.Include("Users").Where(c => c.Animationid == aid).OrderByDescending(c => c.Likenum).ToList();
            return comm;
        }
        #endregion

        #region 返回动漫简介
        public string GetAPlot(int id)
        {
            return db.Animation.Find(id).Aplot;
        }
        #endregion

        #region 添加动漫短评bool
        public bool AddComment(ShortComment sc)
        {
            db.ShortComment.Add(sc);
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

        #region 短评点赞
        public string AddCommentLike(int id, string name)
        {
            db.LikeOrDislike(1, id, name, DateTime.Now);
            return db.ShortComment.Find(id).Likenum.ToString();
        }
        #endregion

        public IEnumerable<Animation> GetAnimations()
        {
            throw new NotImplementedException();
        }
    }
}
