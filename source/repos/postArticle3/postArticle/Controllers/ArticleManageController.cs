using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using postArticle.viewmodel;
using System.Web;
using System.IO;
using System.Web.UI;
using PagedList;
using HtmlAgilityPack;
using System.Data.Entity.Validation;

namespace postArticle.Controllers
{
    public class ArticleManageController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities3 db = new healingForestEntities3();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);

        #endregion
        // -----------------------------------------===============================

        #region ===是否自己的文章====
        private bool IsUserArticle(int id)
        {
            int userID = GetUserID();
            var queryArticleList = from Articledb in db.Article
                                   where Articledb.UserID == userID && Articledb.ArticleID == id
                                   select Articledb;

            return queryArticleList.Any();
        }

        private bool IsCollect(int id)
        {
            int userID = GetUserID();
            var queryArticleList = from Articledb in db.Collect
                                   where Articledb.UserID == userID && Articledb.ArticleID == id
                                   select Articledb;

            return queryArticleList.Any();
        }
        #endregion

        #region ===收藏管理===
        //取消收藏文章
        public ActionResult CancelCollect(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article article = db.Article.Find(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            else if (!CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            else
            {

                var UserID = GetUserID();
                var queryCollectSQL = from Collectdb in db.Collect.Include(c => c.Article).Include(c => c.UserManage)
                                      where Collectdb.UserID == UserID && Collectdb.ArticleID == article.ArticleID
                                      select Collectdb;

                if (queryCollectSQL.Any())
                {
                    article.Likes--;
                    db.Entry(article).CurrentValues.SetValues(article);

                    db.Collect.RemoveRange(queryCollectSQL);
                    db.SaveChanges();
                }


                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString, new { page });

            }
        }


        //進行收藏文章
        public ActionResult ArticleCollect(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article article = db.Article.Find(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            else if (!CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            else
            {
                #region ===存資料到collect===
                var UserID = GetUserID();
                var queryCollectSQL = from Collectdb in db.Collect.Include(c => c.Article).Include(c => c.UserManage)
                                      where Collectdb.UserID == UserID && Collectdb.ArticleID == article.ArticleID
                                      select Collectdb;

                if (!queryCollectSQL.Any())
                {
                    Collect collect = new Collect();
                    collect.UserID = UserID;
                    collect.ArticleID = article.ArticleID;
                    collect.Time = DateTime.Now;

                    article.Likes++;
                    db.Entry(article).CurrentValues.SetValues(article);
                    db.Collect.Add(collect);
                    db.SaveChanges();
                }
                #endregion



                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString, new { page });
            }

        }
        #endregion
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


        //---------------文章內容與留言-----------------------------------------

        public ActionResult ArticleDetails(int? id, int? page)
        {
            //-----分頁-----
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            //
            bool isUser;
            bool isCreatedByUser;
            List<Message> queryMessList = new List<Message>();
            string Display = "none";
            //---------------判斷文章是否存在-----------------------------------------

            #region 判斷文章是否存在
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            // -----------------------------------------===============================

            #region ===是否有登入===
            isUser = false;
            if (CheckLoggedIn())
            {
                isUser = true;
            }
            #endregion

            //---------------該文章底下的留言-----------------------------------------
            queryMessList = (from Messagedb in db.Message.Include(m => m.Article).Include(m => m.UserManage)
                             where Messagedb.ArticleID == id
                             select Messagedb).ToList();
            //-----------------------------------------===============================

            //判斷是不是自己的文章----------------------
            isCreatedByUser = IsUserArticle((int)id);
            //-----------------------------------------===============================

            if (!string.IsNullOrEmpty(TempData["Display"] as string))
            {
                Display = TempData["Display"] as string;
            }

            //---------------建立viewModel-----------------------------------------
            ArticleDetailsViewModel articleDetailsViewModel = new ArticleDetailsViewModel
            {
                article = article,
                messages = queryMessList.ToPagedList(pageNumber, pageSize),
                Page = pageNumber,
                isUser = isUser,
                IsCollect=IsCollect(article.ArticleID),
                isCreatedByUser = isCreatedByUser,
                Display = Display
            };
            //-----------------------------------------===============================

            return View(articleDetailsViewModel);
        }






        //---------------文章編輯-----------------------------------------
        #region ===文章編輯====
        public ActionResult ArticleEdit(int? id)
        {
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================

            if (CheckLoggedIn())
            {
                if (IsUserArticle((int)id))
                {
                    TempData["ArticleID"] = id;

                    ArticleManageViewModel viewModel = new ArticleManageViewModel();
                    viewModel.article = article;
                    return View(viewModel);
                }
            }

            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(ArticleManageViewModel articleManageViewModel, HttpPostedFileBase file)
        {
            int? id = (int)TempData["ArticleID"];
            TempData["ArticleID"] = id;
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================

            if (CheckLoggedIn() && IsUserArticle((int)id))
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        //刪除文件
                        var path = Path.Combine(Server.MapPath("~/Uploads"), articleManageViewModel.article.ImageURL);
                        if (System.IO.File.Exists(path) && articleManageViewModel.article.ImageURL != "index.png")
                        {

                            System.IO.File.Delete(path);
                        }
                        string processedContent = ProcessHtmlContent(articleManageViewModel.article.Content);
                        articleManageViewModel.article.Content = processedContent;

                        var fileName = Guid.NewGuid().ToString() + ".png";
                        path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        file.SaveAs(path);

                        articleManageViewModel.article.ImageURL = fileName;
                    }

                    db.Entry(article).CurrentValues.SetValues(articleManageViewModel.article);
                    db.SaveChanges();


                    return RedirectToAction("ArticleDetails", "ArticleManage", new { id });
                }
                else
                {
                    return View(articleManageViewModel);
                }
            }


            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);

        }
        #endregion
        // -----------------------------------------===============================

        public string ProcessHtmlContent(string htmlContent)
        {
            // 創建 HtmlDocument 實例
            HtmlDocument htmlDocument = new HtmlDocument();

            // 載入 HTML 內容
            htmlDocument.LoadHtml(htmlContent);

            // 選擇要處理的 HTML 標籤，例如 <p>
            var paragraphs = htmlDocument.DocumentNode.Descendants("p");

            // 遍歷每個 <p> 標籤
            foreach (var paragraph in paragraphs)
            {
                // 在這裡可以進行適當的處理，例如對內容進行分段
                // 這裡只是一個示例，你可以根據具體需求進行處理

                // 在每個 <p> 標籤後面插入分段符號 <br>
                paragraph.InnerHtml += "<br/>";
            }

            // 獲取處理後的 HTML 內容
            string processedContent = htmlDocument.DocumentNode.OuterHtml;

            // 返回處理後的 HTML 內容
            return processedContent;
        }



        public ActionResult ArticleDelete(int? id)
        {
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================


            if (CheckLoggedIn())
            {
                if (IsUserArticle((int)id))
                {
                    //刪除文件
                    var path = Path.Combine(Server.MapPath("~/Uploads"), article.ImageURL);
                    if (System.IO.File.Exists(path) && article.ImageURL != "index.png")
                    {
                        System.IO.File.Delete(path);
                    }
                    //


                    db.Article.Remove(article);
                    db.SaveChanges();
                }
            }

            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
        }



        //發布文章
        public ActionResult ArticlePost()
        {
            if (CheckLoggedIn())
            {
                ArticleManageViewModel viewModel = new ArticleManageViewModel();
                return View(viewModel);

            }
            else
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
        }

       

        //發布文章post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticlePost([Bind (Include ="IsChecked")] ArticleManageViewModel articlePost, HttpPostedFileBase file)
        {
            try
            {
                if (CheckLoggedIn())
                {
                    if (ModelState.IsValid)
                    {
                        #region ===搜尋userID===
                        var UserID = Convert.ToInt32(Session["UserID"]);
                        var queryUserSQL = from UserManagedb in db.UserManage.Include(u => u.ThanksfulThings)
                                           where UserManagedb.UserID == UserID
                                           select new
                                           {
                                               UserManagedb.UserType
                                           };


                        var user = queryUserSQL.FirstOrDefault();
                        if (user != null)
                        {

                            if (user.UserType.Equals("Expert") && articlePost.IsChecked)
                            {
                                articlePost.article.ArticleType = "是";
                            }
                            else
                            {
                                articlePost.article.ArticleType = "否";
                            }
                        }
                        #endregion


                        #region ===設置articlePost的資料======
                        //新增文件
                        articlePost.article.ImageURL = "index.png";
                        // 获取当前时间
                        articlePost.article.Time = DateTime.Now;

                        articlePost.article.UserID = UserID;
                        #endregion


                        #region ===上傳圖片====
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + ".png";
                            var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                            file.SaveAs(path);

                            articlePost.article.ImageURL = fileName;
                        }
                        #endregion
                        //

                        db.Article.Add(articlePost.article);
                        db.SaveChanges();
                    }
                    else
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors);
                        var viewModel = new ArticleManageViewModel
                        {
                            article = articlePost.article
                        };

                        return View(viewModel);
                    }
                }

                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                        var errorMessage = $"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                        // 这里可以输出错误消息或记录到日志中，以便进一步调试
                    }
                }
                throw; // 将异常继续抛出以中断程序流程
            }





        }
    }
}

