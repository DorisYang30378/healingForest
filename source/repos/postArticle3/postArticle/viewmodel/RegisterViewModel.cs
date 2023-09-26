using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class RegisterViewModel
    {
        public string ConfirmPassword { get; set; }
        public string code { get; set; }
        public bool rememberPassword { get; set; }
        public UserManage userManage { get; set; }
    }
}