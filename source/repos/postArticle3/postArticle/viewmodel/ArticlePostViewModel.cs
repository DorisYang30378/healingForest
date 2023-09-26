using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace postArticle.viewmodel
{
    public class ArticleManageViewModel
    {
        public Article article { get; set; }
        public UserManage userManage { get; set; }


        [Display(Name = "專業文章")]
        public bool IsChecked { get; set; }



        public string[] Classification =
        {
            "工作","健康","學業","家庭","經濟"
        };

    }
}

