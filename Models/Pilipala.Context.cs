﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PiliPalaEntities : DbContext
    {
        public PiliPalaEntities()
            : base("name=PiliPalaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Animation> Animation { get; set; }
        public virtual DbSet<AnimationPhoto> AnimationPhoto { get; set; }
        public virtual DbSet<Buzzword> Buzzword { get; set; }
        public virtual DbSet<BWdislike> BWdislike { get; set; }
        public virtual DbSet<BWlike> BWlike { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<dongtai> dongtai { get; set; }
        public virtual DbSet<dongtaiComment> dongtaiComment { get; set; }
        public virtual DbSet<dongtaiCommentlike> dongtaiCommentlike { get; set; }
        public virtual DbSet<dongtaiCommentReply> dongtaiCommentReply { get; set; }
        public virtual DbSet<dongtailike> dongtailike { get; set; }
        public virtual DbSet<EClike> EClike { get; set; }
        public virtual DbSet<EComment> EComment { get; set; }
        public virtual DbSet<ECommentReply> ECommentReply { get; set; }
        public virtual DbSet<Edislike> Edislike { get; set; }
        public virtual DbSet<Elike> Elike { get; set; }
        public virtual DbSet<Evaluation> Evaluation { get; set; }
        public virtual DbSet<ExamineShortcomment> ExamineShortcomment { get; set; }
        public virtual DbSet<Follow> Follow { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Mlike> Mlike { get; set; }
        public virtual DbSet<PublishDynamic> PublishDynamic { get; set; }
        public virtual DbSet<PublishEvaluation> PublishEvaluation { get; set; }
        public virtual DbSet<ShortComment> ShortComment { get; set; }
        public virtual DbSet<SLike> SLike { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<UserLikeA> UserLikeA { get; set; }
        public virtual DbSet<UserLikeE> UserLikeE { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersInfo> UsersInfo { get; set; }
        public virtual DbSet<Watch> Watch { get; set; }
        public virtual DbSet<AnimationCategory> AnimationCategory { get; set; }
        public virtual DbSet<tempRecommendAnimation> tempRecommendAnimation { get; set; }
        public virtual DbSet<tempCommunityDongtai> tempCommunityDongtai { get; set; }
        public virtual DbSet<tempCommunityEvaluation> tempCommunityEvaluation { get; set; }
        public virtual DbSet<tempCommunityShortComment> tempCommunityShortComment { get; set; }
        public virtual DbSet<tempEvaluationList> tempEvaluationList { get; set; }
        public virtual DbSet<tempRankingList> tempRankingList { get; set; }
        public virtual DbSet<tempSearchAnimation> tempSearchAnimation { get; set; }
        public virtual DbSet<tempSearchEvaluation> tempSearchEvaluation { get; set; }
        public virtual DbSet<tempSearchUsers> tempSearchUsers { get; set; }
        public virtual DbSet<tempDongtai> tempDongtai { get; set; }
        public virtual DbSet<tempEvaluation> tempEvaluation { get; set; }
        public virtual DbSet<tempFans> tempFans { get; set; }
        public virtual DbSet<tempFollow> tempFollow { get; set; }
        public virtual DbSet<tempShortComment> tempShortComment { get; set; }
        public virtual DbSet<BanSpeech> BanSpeech { get; set; }
    
        public virtual int CommunitySet(string personName)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CommunitySet", personNameParameter);
        }
    
        public virtual int EvaluationListSet(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("EvaluationListSet", idParameter);
        }
    
        public virtual int FollowOrCancelFollow(string follower, string userName, Nullable<System.DateTime> time)
        {
            var followerParameter = follower != null ?
                new ObjectParameter("Follower", follower) :
                new ObjectParameter("Follower", typeof(string));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var timeParameter = time.HasValue ?
                new ObjectParameter("Time", time) :
                new ObjectParameter("Time", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("FollowOrCancelFollow", followerParameter, userNameParameter, timeParameter);
        }
    
        public virtual int LikeOrDislike(Nullable<int> num, Nullable<int> id, string userName, Nullable<System.DateTime> time)
        {
            var numParameter = num.HasValue ?
                new ObjectParameter("num", num) :
                new ObjectParameter("num", typeof(int));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var timeParameter = time.HasValue ?
                new ObjectParameter("Time", time) :
                new ObjectParameter("Time", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("LikeOrDislike", numParameter, idParameter, userNameParameter, timeParameter);
        }
    
        public virtual int PersonalSpace(string personName, string visitor)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            var visitorParameter = visitor != null ?
                new ObjectParameter("Visitor", visitor) :
                new ObjectParameter("Visitor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PersonalSpace", personNameParameter, visitorParameter);
        }
    
        public virtual int RankingListSet(string name, Nullable<int> type)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RankingListSet", nameParameter, typeParameter);
        }
    
        public virtual int RecommendSet(string personName)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RecommendSet", personNameParameter);
        }
    
        public virtual int SearchSet(string keyWord, string personName)
        {
            var keyWordParameter = keyWord != null ?
                new ObjectParameter("KeyWord", keyWord) :
                new ObjectParameter("KeyWord", typeof(string));
    
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SearchSet", keyWordParameter, personNameParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int UserRegister(Nullable<int> num, string email, string userName, string pwd)
        {
            var numParameter = num.HasValue ?
                new ObjectParameter("num", num) :
                new ObjectParameter("num", typeof(int));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var pwdParameter = pwd != null ?
                new ObjectParameter("Pwd", pwd) :
                new ObjectParameter("Pwd", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UserRegister", numParameter, emailParameter, userNameParameter, pwdParameter);
        }
    
        public virtual int WatchUpdate(Nullable<int> id, string type, string name, Nullable<System.DateTime> time)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var timeParameter = time.HasValue ?
                new ObjectParameter("time", time) :
                new ObjectParameter("time", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WatchUpdate", idParameter, typeParameter, nameParameter, timeParameter);
        }
    
        public virtual int TouristSpace(string personName, string visitor)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            var visitorParameter = visitor != null ?
                new ObjectParameter("Visitor", visitor) :
                new ObjectParameter("Visitor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("TouristSpace", personNameParameter, visitorParameter);
        }
    
        public virtual int GetDongtai(string personName)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetDongtai", personNameParameter);
        }
    
        public virtual int GetEvaluation(string personName)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetEvaluation", personNameParameter);
        }
    
        public virtual int GetFans(string personName, string visitor)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            var visitorParameter = visitor != null ?
                new ObjectParameter("Visitor", visitor) :
                new ObjectParameter("Visitor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetFans", personNameParameter, visitorParameter);
        }
    
        public virtual int GetFollow(string personName, string visitor)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            var visitorParameter = visitor != null ?
                new ObjectParameter("Visitor", visitor) :
                new ObjectParameter("Visitor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetFollow", personNameParameter, visitorParameter);
        }
    
        public virtual int GetShortComment(string personName)
        {
            var personNameParameter = personName != null ?
                new ObjectParameter("PersonName", personName) :
                new ObjectParameter("PersonName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetShortComment", personNameParameter);
        }
    }
}
