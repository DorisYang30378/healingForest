using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Linq;
using System.Web.Mvc;


namespace postArticle.Controllers
{
    public class SignInController : Controller
    {
        #region 基礎屬性
        private healingForestEntities2 db = new healingForestEntities2();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);
        #endregion


        #region ===登入功能===
        public ActionResult login()
        {
            #region ===已經登入進入首頁====
            if (CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            #endregion

            #region ===未登入則停在登入頁面====
            else
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                return View(loginViewModel);
            }
            #endregion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                #region ===判斷資料庫中是否有該暱稱和密碼===
                var querySQL = from UserManagedb in db.UserManage
                               where UserManagedb.Password == loginViewModel.password && UserManagedb.UserName == loginViewModel.username
                               select new
                               {
                                   UserManagedb.UserID
                               };
                #endregion

                #region ===如果有進入首頁===
                if (querySQL.Any())
                {
                    var user = querySQL.FirstOrDefault();
                    Session["UserID"] = user.UserID;

                    return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
                }
                #endregion

                #region ===如果沒有顯示錯誤訊息===
                else
                {
                    loginViewModel.ErrMessage = "帳號不存在或密碼錯誤";
                    return View(loginViewModel);
                }
                #endregion
            }
            return View(loginViewModel);
        }
        #endregion


        #region ===登出功能===
        public ActionResult Logout()
        {
            // 清除session
            Session["UserID"] = null;

            // 返回主畫面
            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
        }
        #endregion
    }
}