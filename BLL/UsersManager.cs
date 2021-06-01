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
    public class UsersManager
    {
        readonly IUsers iuser = DataAccess.CreateUsers();


        #region 获取用户信息
        public IEnumerable<Users> GetUsers()
        {
            return iuser.GetUsers();
        }
        #endregion

        #region 获取某个用户 两个重载（id、用户名）
        public Users GetUser(int id)
        {
            return iuser.GetUsersById(id);
        }

        public Users GetUser(string name)
        {
            return iuser.GetUsersByName(name);
        }
        #endregion

        #region 获取用户信息 两个重载
        public UsersInfo GetUsersInfo(int id)
        {
            return iuser.GetUsersInfo(id);
        }

        public UsersInfo GetUsersInfo(string name)
        {
            return iuser.GetUsersInfo(name);
        }

        #endregion

        #region 删除用户 重载（id、用户名）
        public bool RemoveUser(Users user)
        {
            return iuser.RemoveUser(user);
        }

        public bool RemoveUser(int id)
        {
            return iuser.RemoveUserById(id);
        }
        #endregion

        #region 判断用户是否唯一
        public bool IsUsernameUnique(string name)
        {
            return iuser.IsUsernameUnique(name);
        }
        #endregion

        #region 获取用户发布的测评
        public IEnumerable<tempEvaluation> GetUserEvaluations(string name)
        {
            return iuser.GetUserEvaluations(name);
        }
        #endregion

        #region 获取用户发布短评
        public IEnumerable<tempShortComment> GetUserShortComment(string name)
        {
            return iuser.GetUsersShortComment(name);
        }
        #endregion

        #region 获取用户发布短评
        public IEnumerable<tempDongtai> GetUserDongtai(string name)
        {
            return iuser.GetUserDongtais(name);
        }
        #endregion

        #region 修改用户信息 乱七八糟的都有
        public bool EditUsersInfo(string name, string pic)
        {
            return iuser.EditUserInfos(name, pic);
        }

        public bool EditUserInfos(string name, string gender, DateTime? birthday, string signatures)
        {
            return iuser.EditUserInfos(name, gender, birthday, signatures);
        }

        public string EditPassword(string oldPwd, string newPwd, string name, string salt)
        {
            return iuser.EditPassword(oldPwd, newPwd, name, salt);
        }
        #endregion

        /*-----------------------------------------------------------------*/

        #region 判断邮箱是否唯一
        public bool IsEmailUnique(string name)
        {
            return iuser.IsEmailUnique(name);
        }
        #endregion

        #region 根据邮箱获取用户名
        public string EmailToName(string email)
        {
            return iuser.EmailToName(email);
        }
        #endregion

        #region 用户注册
        public string Register(string username, string pwd, string email, string salt)
        {
            string data;
            if (iuser.IsUsernameUnique(username) == false)
            {
                int v = iuser.AddUser(username, pwd, email, salt);
                if (v == 1)
                    data = "success";
                else if (v == -2)
                    data = "illEmail";  //-2代表邮箱已存在
                else
                    data = "fail";
            }
            // 用户名已存在时返回 "illegalname"
            else
            {
                data = "illegalname";
            }
            return data;
        }
        #endregion

        #region 重设密码
        public string ResetPwd(string name, string pwd, string salt)
        {
            return iuser.ResetPwd(name, pwd, salt);
        }
        #endregion


        #region 获取用户的浏览记录
        public IEnumerable<Watch> GetUserHistory(string name)
        {
            return iuser.GetUserHistory(name);
        }
        #endregion

        #region 分布视图获取用户浏览记录中的动漫
        public Animation GetUserHistoryAnimation(int id)
        {
            return iuser.GetUserHistoryAnimation(id);
        }
        #endregion

        #region 分布视图获取用户浏览记录的测评
        public Evaluation GetUserHistoryEvaluation(int id)
        {

            return iuser.GetUserHistoryEvaluation(id);
        }
        #endregion


        #region 获取用户的关注信息
        public IEnumerable<tempFollow> GetUserFollows(int start, string name, string visitor)
        {
            return iuser.GetUserFollow(start, name, visitor);
        }
        #endregion

        #region 获取用户的粉丝信息
        public IEnumerable<tempFans> GetUserFans(int start, string name, string visitor)
        {
            return iuser.GetUserFans(start, name, visitor);
        }
        #endregion

        #region 关注用户或取消关注用户
        public bool Following(int id, string name)
        {
            return iuser.Following(id, name);
        }
        #endregion

        #region 获取游客发布的测评
        public IEnumerable<tempEvaluation> GetTouristEvaluations(string name)
        {
            return iuser.GetTouristEvaluations(name);
        }
        #endregion

        #region 获取游客发布短评
        public IEnumerable<tempShortComment> GetTouristShortComment(string name)
        {
            return iuser.GetTouristShortComment(name);
        }
        #endregion

        #region 获取游客发布短评
        public IEnumerable<tempDongtai> GetTouristDongtai(string name)
        {
            return iuser.GetTouristDongtai(name);
        }
        #endregion

        #region 获取游客的关注信息
        public IEnumerable<tempFollow> GetTouristFollow(int start, string name, string visitor)
        {
            return iuser.GetTouristFollow(start, name, visitor);
        }
        #endregion

        #region 获取游客的粉丝信息
        public IEnumerable<tempFans> GetTouristFans(int start, string name, string visitor)
        {
            return iuser.GetTouristFans(start, name, visitor);
        }
        #endregion


        #region 比较密码正确性     用户登录
        public bool ComparePwd(string value, string name)
        {
            return iuser.ComparePwd(value, name);
        }
        #endregion

        #region 获取用户的盐    用户登录
        public string GetSalt(string name)
        {
            return iuser.GetSalt(name);
        }
        #endregion

    }
}
