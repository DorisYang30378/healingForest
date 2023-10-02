using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace postArticle.Controllers
{
    public class MessageManageController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities3   db = new healingForestEntities3();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);

        #endregion
        // -----------------------------------------===============================

        private Article CheckArticleExists(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Article article = db.Article.Find(id);

            if (article == null)
            {
                return null;
            }

            return article;
        }

        public ActionResult CreateMessage(ArticleDetailsViewModel articleDetailsViewModel, int? id, int? page)
        {
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion

            articleDetailsViewModel.Display = "block";

            if (CheckLoggedIn())
            {
                var UserID = GetUserID();

                if (!string.IsNullOrEmpty(articleDetailsViewModel.Content))
                {
                    if (ModelState.IsValid)
                    {
                        Message message = new Message
                        {
                            ArticleID = (int)id,
                            Content = articleDetailsViewModel.Content,
                            Time = DateTime.Now,
                            UserID = UserID
                        };

                        articleDetailsViewModel.Display = "none";

                        db.Message.Add(message);
                        db.SaveChanges();
                    }
                }
            }

            TempData["Display"] = articleDetailsViewModel.Display;
            return RedirectToAction("ArticleDetails", "ArticleManage", new { id, page });
        }
    }
}