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
    
    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            this.Mlike = new HashSet<Mlike>();
        }
    
        public int Messageid { get; set; }
        public string UserName { get; set; }
        public int Buzzwordid { get; set; }
        public string Content { get; set; }
        public System.DateTime TIme { get; set; }
        public Nullable<int> Likenum { get; set; }
    
        public virtual Buzzword Buzzword { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mlike> Mlike { get; set; }
    }
}