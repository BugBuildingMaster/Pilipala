using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace IDAL
{
    public interface IUsers
    {
        //获取所有用户
        IEnumerable<Users> GetUsers();
        //拿到用户信息
        UsersInfo GetUsersInfo(string name);
        UsersInfo GetUsersInfo(int id);
        //通过id获取单个用户信息
        Users GetUsersById(int id);
        //通过姓名获取单个用户信息
        Users GetUsersByName(string name);
        //判断用户是否存在
        bool IsUsernameUnique(string name);

        //修改某用户
        bool EditUser(Users user);
        //删除某用户
        bool RemoveUser(Users user);
        //根据id删除用户
        bool RemoveUserById(int id);
        //获取某个用户发布的测评
        IEnumerable<tempEvaluation> GetUserEvaluations(string name);
        //获取某个用户发布的动态
        IEnumerable<tempDongtai> GetUserDongtais(string name);
        IEnumerable<tempShortComment> GetUsersShortComment(string name);
        //修改用户信息
        bool EditUserInfos(string name, string portrait);
        bool EditUserInfos(string name, string gender, DateTime? birthday, string signatures);
        // 修改密码
        string EditPassword(string oldPwd, string newPwd, string name,string salt);


        /*-------------------------------------------------------------------*/
        bool IsEmailUnique(string email);    //判断邮箱是否存在
        //添加某用户
        int AddUser(string username, string pwd, string email, string salt);
        IEnumerable<Watch> GetUserHistory(string name);    //获取用户浏览记录
        Animation GetUserHistoryAnimation(int id);    //获取用户浏览记录中的动漫
        Evaluation GetUserHistoryEvaluation(int id);    //获取用户浏览记录中的测评
        IEnumerable<tempFollow> GetUserFollow(int start, string name, string visitor);    //获取用户关注信息
        IEnumerable<tempFans> GetUserFans(int start, string name, string visitor);    //获取用户粉丝信息
        bool Following(int id, string name);      //关注或取消关注
        IEnumerable<tempEvaluation> GetTouristEvaluations(string name);  //获取游客发布的测评
        IEnumerable<tempShortComment> GetTouristShortComment(string name);      //获取游客发布短评
        IEnumerable<tempDongtai> GetTouristDongtai(string name);        //获取游客发布动态
        IEnumerable<tempFollow> GetTouristFollow(int start, string name, string visitor);    //获取游客关注信息
        IEnumerable<tempFans> GetTouristFans(int start, string name, string visitor);    //获取游客粉丝信息
        bool ComparePwd(string value,string name);  //比较密码正确性
        string GetSalt(string name);    //获取用户的盐

    }
}
