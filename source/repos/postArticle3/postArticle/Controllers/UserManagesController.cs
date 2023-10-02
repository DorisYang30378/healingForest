using _Platform.Service;
using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace postArticle.Controllers
{
    public class UserManagesController : Controller
    {
        #region 基礎屬性    
        private healingForestEntities3 db = new healingForestEntities3();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);
        #endregion

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(RegisterViewModel registerViewModel)
        {
            if (IsValidUser(registerViewModel.userManage.Account, registerViewModel.userManage.Password))
            {
                var querySQL = from UserManagedb in db.UserManage
                               where UserManagedb.Password == registerViewModel.userManage.Password && UserManagedb.Account == registerViewModel.userManage.Account
                               select new
                               {
                                   UserManagedb.UserID
                               };


                #region ===如果有進入首頁===
                if (querySQL.Any())
                {
                    var user = querySQL.FirstOrDefault();
                    Session["UserID"] = user.UserID;

                    return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
                }
                #endregion
            }

            ModelState.AddModelError("Password", "帳號或密碼並不存在!");
            return View();

        }

        private bool IsValidUser(string account, string password)
        {
            UserManage user = db.UserManage.FirstOrDefault(u => u.Account == account);

            if (user != null)
            {
                // 验证密码是否匹配
                if (user.Password == password)
                {
                    return true;
                }
            }

            return false;
        }

        // 发送验证码
        [HttpPost]
        public ActionResult SendVerificationCode(string email)
        {
            MaillService ms = new MaillService();
            try
            {
                string subject = "註冊驗證碼";
                var verificationCode = GenerateRandomCode(4);
                string body = @"
                        <p>您的驗證碼為:{verificationCode}
                        ======================================================<br />
                        此為系統自動發送的電子郵件，請勿直接回覆本信件。<br />
                        ======================================================<br />
                        </p>
                        ";
                body = body.Replace("{verificationCode}", verificationCode);
                ms.SendMail(email, subject, body);
                Session["VerificationCode"] = verificationCode;
            }
            catch (Exception)
            {
                // 发送失败的处理逻辑
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        private string GenerateRandomCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            var code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }


        // GET: UserManages/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.ThanksfulThings, "UserID", "Content");
            return View();
        }
        // POST: UserManages/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel registerViewModel)
        {
            if (registerViewModel != null && registerViewModel.userManage.Password.Equals(registerViewModel.ConfirmPassword))
            {
                // 验证验证码是否正确
                var sessionVerificationCode = Session["VerificationCode"] as string;
                if (registerViewModel.code == sessionVerificationCode)
                {
                    // 密码匹配，继续保存用户
                    // 添加您的保存用户逻辑

                    registerViewModel.userManage.Experience = 0.0;
                    registerViewModel.userManage.LevelValue = 0;
                    registerViewModel.userManage.UserType = "member";


                    db.UserManage.Add(registerViewModel.userManage);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }


            return View();
        }


        public ActionResult forget_password()
        {
            return View();
        }
        [HttpPost]
        public ActionResult forget_password(string account, string email)
        {
            if (IsValidPassword(account, email))
            {
                MaillService ms = new MaillService();
                try
                {
                    UserManage user = db.UserManage.FirstOrDefault(u => u.Account == account);
                    string subject = "忘記密碼";
                    string body = @"
                        <p>您的密碼為: {Password} </p>
                        ======================================================<br />
                        此為系統自動發送的電子郵件，請勿直接回覆本信件。<br />
                        ======================================================<br />
                        </p>
                        ";
                    body = body.Replace("{Password}", user.Password);
                    ms.SendMail(email, subject, body);

                }
                catch (Exception)
                {
                    // 发送失败的处理逻辑
                    return Json(new { success = false, message = "發送錯誤" });
                }
                return RedirectToAction("login");
            }
            else
            {
                ModelState.AddModelError("Email", "帳號與Email並不匹配!");
                return View();
            }
        }
        private bool IsValidPassword(string account, string email)
        {
            UserManage user = db.UserManage.FirstOrDefault(u => u.Account == account);

            if (user != null)
            {
                // Email是否匹配
                if (user.Email == email)
                {
                    return true;
                }
            }

            return false;
        }
    }
}