//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsersInfo
    {
        public string UserName { get; set; }
        public string Portrait { get; set; }
        public string Gender { get; set; }
        public string Signatures { get; set; }
        public int Fans { get; set; }
        public int Fowllors { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
