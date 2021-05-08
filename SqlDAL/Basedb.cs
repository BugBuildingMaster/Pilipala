using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace SqlDAL
{
    public class Basedb
    {
        public PiliPalaEntities db
        {
            get
            {  //从当前线程中获取 MvcBookStoreEntities对象
                PiliPalaEntities db = CallContext.GetData("DB") as PiliPalaEntities;
                if (db == null)
                {
                    db = new PiliPalaEntities();
                    CallContext.SetData("DB", db);
                }
                return db;
            }
        }
    }
}
