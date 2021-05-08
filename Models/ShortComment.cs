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
    
    public partial class ShortComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShortComment()
        {
            this.ExamineShortcomment = new HashSet<ExamineShortcomment>();
            this.SLike = new HashSet<SLike>();
        }
    
        public int Scommentid { get; set; }
        public int Animationid { get; set; }
        public string UserName { get; set; }
        public System.DateTime Time { get; set; }
        public string content { get; set; }
        public Nullable<double> Score { get; set; }
        public Nullable<int> Likenum { get; set; }
    
        public virtual Animation Animation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamineShortcomment> ExamineShortcomment { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SLike> SLike { get; set; }
    }
}
