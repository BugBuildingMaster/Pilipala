using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using IDAL;

namespace SqlDAL
{
    public class SqlServerUsers : Basedb, IUsers
    {
        #region 添加用户
        public bool AddUser(Users user)
        {
            db.Users.Add(user);
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

        #region 修改用户
        public bool EditUser(Users user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 获取用户信息
        public IEnumerable<Users> GetUsers()
        {
            var users = db.Users.ToList();
            return users;
        }
        #endregion

        #region 通过ID获取用户
        public Users GetUsersById(int id)
        {
            string name = db.Users.Where(c => c.Userid == id).FirstOrDefault().UserName;
            Users user = db.Users.Find(name);
            return user;
        }
        #endregion

        #region 通过Name获取用户
        public Users GetUsersByName(string name)
        {
            Users user = db.Users.Where(x => x.UserName == name).FirstOrDefault();
            return user;
        }

        #endregion

        #region 删除某用户
        public bool RemoveUser(Users user)
        {
            db.Users.Remove(user);
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

        #region 根据id删除某用户
        public bool RemoveUserById(int id)
        {
            Users user = this.GetUsersById(id);
            return this.RemoveUser(user);
        }
        #endregion

        #region 获取用户信息
        public UsersInfo GetUsersInfo(string name)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.UsersInfo.Find(name);
        }
        public UsersInfo GetUsersInfo(int id)
        {
            string name = db.Users.Where(a => a.Userid == id).FirstOrDefault().UserName;
            return db.UsersInfo.Find(name);
        }
        #endregion

        #region 获取用户发布测评
        public IEnumerable<tempEvaluation> GetUserEvaluations(string name)
        {
            db.GetEvaluation(name);
            return db.tempEvaluation.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 获取用户发布动态
        public IEnumerable<tempDongtai> GetUserDongtais(string name)
        {
            db.GetDongtai(name);
            return db.tempDongtai.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 获取用户发布短评
        public IEnumerable<tempShortComment> GetUsersShortComment(string name)
        {
            db.GetShortComment(name);
            return db.tempShortComment.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 修改用户信息
        public bool EditUserInfos(string name, string portrait)
        {
            db.UsersInfo.Find(name).Portrait = portrait;
            return db.SaveChanges() > 0;
        }

        public bool EditUserInfos(string name, string gender, DateTime? birthday, string signatures)
        {
            db.UsersInfo.Find(name).Gender = gender;
            db.UsersInfo.Find(name).Birthday = birthday;
            db.UsersInfo.Find(name).Signatures = signatures;

            return db.SaveChanges() > 0;
        }

        public string EditPassword(string oldPwd, string newPwd, string name, string salt)
        {

            db.Users.Find(name).Pwd = newPwd;
            db.Users.Find(name).Salt = salt;
            int change = db.SaveChanges();
            if (change > 0)
            {
                return "success";
            }
            else if (change == 0)
            {
                return "re";
            }
            else
            {
                return "fail";
            }
        }
        #endregion

        /*--------------------------------------------------------------*/
        #region 判断用户名是否唯一
        public bool IsUsernameUnique(string name)
        {
            bool exists = db.Users.Any(x => x.UserName == name);
            return exists;
        }
        #endregion

        #region 获取用户浏览记录
        public IEnumerable<Watch> GetUserHistory(string name)
        {
            return db.Watch.Where(e => e.UserName == name).OrderByDescending(o => o.Time).ToList();
        }
        #endregion

        #region 获取用户动漫浏览历史
        public Animation GetUserHistoryAnimation(int id)
        {
            return db.Animation.Where(e => e.Animationid == id).FirstOrDefault();
        }
        #endregion

        #region 获取用户测评浏览历史
        public Evaluation GetUserHistoryEvaluation(int id)
        {
            return db.Evaluation.Where(e => e.Evaluationid == id).FirstOrDefault();
        }
        #endregion

        #region 获取用户关注信息
        public IEnumerable<tempFollow> GetUserFollow(int start, string name, string visitor)
        {
            db.GetFollow(name, visitor);
            return db.tempFollow.OrderByDescending(e => e.Time).Take(start * 20).Skip((start - 1) * 20).ToList();
        }
        #endregion

        #region 获取用户粉丝信息
        public IEnumerable<tempFans> GetUserFans(int start, string name, string visitor)
        {
            db.GetFans(name, visitor);
            return db.tempFans.OrderByDescending(e => e.Time).Take(start * 20).Skip((start - 1) * 20).ToList();
        }
        #endregion

        #region 关注用户或取消关注用户
        public bool Following(int id, string name)
        {
            var FollowName = db.Users.Where(w => w.Userid == id).Select(c => c.UserName).FirstOrDefault().ToString();
            db.FollowOrCancelFollow(FollowName, name, DateTime.Now);
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

        #region 获取游客发布的测评
        public IEnumerable<tempEvaluation> GetTouristEvaluations(string name)
        {
            db.GetEvaluation(name);
            return db.tempEvaluation.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 获取游客发布短评
        public IEnumerable<tempShortComment> GetTouristShortComment(string name)
        {
            db.GetShortComment(name);
            return db.tempShortComment.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 获取游客发布动态
        public IEnumerable<tempDongtai> GetTouristDongtai(string name)
        {
            db.GetDongtai(name);
            return db.tempDongtai.OrderByDescending(e => e.Time);
        }
        #endregion

        #region 获取游客关注信息
        public IEnumerable<tempFollow> GetTouristFollow(int start, string name, string visitor)
        {
            db.GetFollow(name, visitor);
            return db.tempFollow.OrderByDescending(e => e.Time).Take(start * 20).Skip((start - 1) * 20).ToList();
        }
        #endregion

        #region 获取游客粉丝信息
        public IEnumerable<tempFans> GetTouristFans(int start, string name, string visitor)
        {
            db.GetFans(name, visitor);
            return db.tempFans.OrderByDescending(e => e.Time).Take(start * 20).Skip((start - 1) * 20).ToList();
        }
        #endregion

        #region 比较密码正确性
        public bool ComparePwd(string value, string name)
        {
            string TruePwd = db.Users.Find(name).Pwd;
            //位运算比较长度
            int judge = value.Length ^ TruePwd.Length;
            //通过位运算进行慢比较，防止计时攻击，即通过响应时间推断密码
            for (int i = 0; i < value.Length && i < TruePwd.Length; i++)
            {
                judge |= value[i] ^ TruePwd[i];
            }
            return judge == 0;
        }
        #endregion

        #region 获取用户的盐
        public string GetSalt(string name)
        {
            string value = db.Users.Find(name).Salt;
            return value;
        }
        #endregion
    }
}
