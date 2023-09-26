using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace postArticle.viewmodel
{
    public class ArticleIndexViewModel
    {
        public string userName { get; set; }
        public bool isUser { get; set; }

        public string ShowCollect { get; set; }
        public bool IsShowCollect { get; set; }

        public string ShowMyArticle { get; set; }
        public bool IsShowMyArticle { get; set; }


        public string ShowSearch { get; set; }
        public bool IsShowSearch { get; set; }


        public PagedList.IPagedList<Article> articles { get; set; }
        public PagedList.IPagedList<bool> IsCollect { get; set; }


        public int? Page { get;set; }

        public string Classification { get; set;}
        public string Order { get; set;}
    }
}