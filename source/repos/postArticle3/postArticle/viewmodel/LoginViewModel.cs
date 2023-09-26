using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class LoginViewModel
    {
        [Display(Name = "暱稱"), Required]
        public string username { get; set; }

        [Display(Name = "密碼"), Required, DataType(DataType.Password)]
        public string password { get; set; }

        public string ErrMessage { get; set; }

    }

}