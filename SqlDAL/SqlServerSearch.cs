using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Models;

namespace SqlDAL
{
    public class SqlServerSearch : Basedb, ISearch
    {
        public bool Search(string KeyWord, string user)
        {
            string name = user;
            name = name.Replace("'", "''");
            if (KeyWord.Length > 20)       //处理过长的字符
            {
                KeyWord = KeyWord.Substring(0, 20);
            }
            KeyWord = KeyWord.Replace("'", "''");   //防止存储过程出错
            db.SearchSet(KeyWord, name);
            return true;
            /*if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }

        public string GetEvaluationPreview(int id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            return db.Evaluation.Find(id).Content;
        }

        public IEnumerable<tempSearchEvaluation> SearchEvaluation()
        {
            return db.tempSearchEvaluation;
        }

        public IEnumerable<tempSearchAnimation> SearchAnimation()
        {
            return db.tempSearchAnimation;
        }

        public IEnumerable<tempSearchUsers> SearchUsers()
        {
            return db.tempSearchUsers;
        }
    }
}
