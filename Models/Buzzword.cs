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
    
    public partial class Buzzword
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Buzzword()
        {
            this.BWdislike = new HashSet<BWdislike>();
            this.BWlike = new HashSet<BWlike>();
            this.Message = new HashSet<Message>();
        }
    
        public int Buzzwordid { get; set; }
        public string Buzzwordname { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public Nullable<int> Likenum { get; set; }
        public Nullable<int> Dislike { get; set; }
        public Nullable<int> Viewnum { get; set; }
        public Nullable<int> Messagaenum { get; set; }
        public string BimagePic { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BWdislike> BWdislike { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BWlike> BWlike { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Message { get; set; }
    }
}
