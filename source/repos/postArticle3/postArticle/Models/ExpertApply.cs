//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ExpertApply
    {
        public int ExpertApplyID { get; set; }
        public string ExpertField { get; set; }
        public string ExpertInfo { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public string ExpertImgURL { get; set; }
        public string Remark { get; set; }
    
        public virtual UserManage UserManage { get; set; }
    }
}
