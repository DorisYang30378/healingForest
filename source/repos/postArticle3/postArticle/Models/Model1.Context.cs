﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace postArticle.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class healingForestEntities3 : DbContext
    {
        public healingForestEntities3()
            : base("name=healingForestEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Chatroom> Chatroom { get; set; }
        public virtual DbSet<ChatroomLog> ChatroomLog { get; set; }
        public virtual DbSet<Checkin> Checkin { get; set; }
        public virtual DbSet<CheckinRewards> CheckinRewards { get; set; }
        public virtual DbSet<Collect> Collect { get; set; }
        public virtual DbSet<ExpertAnswer> ExpertAnswer { get; set; }
        public virtual DbSet<ExpertApply> ExpertApply { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Mood> Mood { get; set; }
        public virtual DbSet<R_Report> R_Report { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<Report_Message> Report_Message { get; set; }
        public virtual DbSet<ReportMember> ReportMember { get; set; }
        public virtual DbSet<RReportMember> RReportMember { get; set; }
        public virtual DbSet<RReportMessage> RReportMessage { get; set; }
        public virtual DbSet<ThanksfulThings> ThanksfulThings { get; set; }
        public virtual DbSet<UserManage> UserManage { get; set; }
        public virtual DbSet<UserQuestion> UserQuestion { get; set; }
        public virtual DbSet<articleandmessage> articleandmessage { get; set; }
        public virtual DbSet<QA> QA { get; set; }
    }
}
