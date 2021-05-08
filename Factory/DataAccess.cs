using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using System.Reflection;
using System.Configuration;

namespace Factory
{
    public class DataAccess
    {
        private readonly static string AssemblyName = ConfigurationManager.AppSettings["Path"].ToString();
        private readonly static string db = ConfigurationManager.AppSettings["DB"].ToString();

        public static IUsers CreateUsers()
        {
            string classname = AssemblyName + "." + db + "Users";
            return (IUsers)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static IAnimation CreateAnimations()
        {
            string classname = AssemblyName + "." + db + "Animation";
            return (IAnimation)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static ICategory CreateCategory()
        {
            string classname = AssemblyName + "." + db + "Category";
            return (ICategory)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static IEvaluation CreateEvaluation()
        {
            string classname = AssemblyName + "." + db + "Evaluation";
            return (IEvaluation)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static IRecommend CreateRecommend()
        {
            string classname = AssemblyName + "." + db + "Recommend";
            return (IRecommend)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static ISearch CreateSearch()
        {
            string classname = AssemblyName + "." + db + "Search";
            return (ISearch)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static ICommunity CreateCommunity()
        {
            string classname = AssemblyName + "." + db + "Community";
            return (ICommunity)Assembly.Load("SqlDAL").CreateInstance(classname);
        }

        public static IRankinglist CreateRankinglist()
        {
            string classname = AssemblyName + "." + db + "Rankinglist";
            return (IRankinglist)Assembly.Load("SqlDAL").CreateInstance(classname);
        }
    }
}
