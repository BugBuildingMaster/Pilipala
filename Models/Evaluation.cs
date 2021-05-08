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
    
    public partial class Evaluation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Evaluation()
        {
            this.EClike = new HashSet<EClike>();
            this.EComment = new HashSet<EComment>();
            this.Edislike = new HashSet<Edislike>();
            this.Elike = new HashSet<Elike>();
            this.UserLikeE = new HashSet<UserLikeE>();
        }
    
        public int Evaluationid { get; set; }
        public string Aname { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public System.DateTime Time { get; set; }
        public string Content { get; set; }
        public Nullable<int> Likenum { get; set; }
        public Nullable<int> Dislikenum { get; set; }
        public Nullable<double> Score { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EClike> EClike { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EComment> EComment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Edislike> Edislike { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Elike> Elike { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLikeE> UserLikeE { get; set; }
    }
}