using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace postArticle.viewmodel
{
    public class ArticleDetailsViewModel
    { 
        public bool isUser { get; set; }

        public Article article { get; set; }

        public PagedList.IPagedList<Message> messages { get; set; }

        [Display(Name = "留言"), Required, DataType(DataType.Text)]
        public string Content { get; set; }

        public bool isCreatedByUser { get; set; }
        public bool IsCollect { get; set; }

        public int? Page { get; set; }

        public string Display { get; set; }
    }
}