using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Net;
using postArticle.viewmodel;
using System.IO;
using PagedList;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace postArticle.Controllers
{
    public class HomeController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities3 db = new healingForestEntities3();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);
        #endregion
        // -----------------------------------------===============================

        //---------------首頁-----------------------------------------
        #region ===首頁===
        public ActionResult ArticleIndex(ArticleIndexViewModel articleIndexViewModel, int? page)
        {
            //---------------變數-----------------------------------------
            #region ===變數===
            //-------分頁-------
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            //-----文章資料庫-----
            IEnumerable<Article> articles;
            //--------搜尋--------
            bool IsShowSearch = articleIndexViewModel.IsShowSearch;
            string searchString;
            //-------文章分類------
            string ClassificationString;
            string OrderString;
            //--------使用者--------
            int UserID;
            UserManage userManage;
            //--------收藏文章--------
            IQueryable<Collect> queryCollectSQL;
            bool IsShowCollect;
            List<bool> IsCollect;
            //我的文章
            bool IsShowMyArticle;
            #endregion
            // -----------------------------------------===============================

            //---------------訪客畫面-----------------------------------------
            if (!CheckLoggedIn())
            {
                return View(visitor());
            }
            //---------------會員畫面-----------------------------------------
            else
            {
                return View(Member());
            }
            // -----------------------------------------===============================

            //---------------訪客視圖-----------------------------------------
            ArticleIndexViewModel visitor()
            {
                articles = db.Article.Include(a => a.UserManage).ToList();

                #region ===篩選資料===
                filter();
                #endregion

                #region ===將預設值push到viewmodel===
                return new ArticleIndexViewModel
                {
                    userName = "訪客",
                    isUser = false,
                    articles = articles.ToPagedList(pageNumber, pageSize),
                    Page = pageNumber,
                    ShowSearch = searchString
                };
                #endregion
            }
            // -----------------------------------------===============================
            //會員視圖
            ArticleIndexViewModel Member()
            {
                #region ===獲取使用者===
                //將session的值轉成int
                UserID = GetUserID();
                userManage = db.UserManage.Find(UserID);
                #endregion

                #region ===收藏的文章===
                //已收藏的文章列表
                queryCollectSQL = from Collectdb in db.Collect.Include(c => c.Article).Include(c => c.UserManage)
                                  where Collectdb.UserID == UserID
                                  select Collectdb;

                //判斷是否按了我的收藏按鈕
                IsShowCollect = GetBooleanValue(articleIndexViewModel?.ShowCollect, "IsShowCollect");
                TempData["IsShowCollect"] = IsShowCollect;


                if (IsShowCollect)
                {
                    // 將收藏的文章清單賦值給 articles 變數
                    articles = queryCollectSQL.Select(c => c.Article).ToList();
                }
                else
                {
                    articles = db.Article.Include(a => a.UserManage).ToList();
                }
                #endregion

                #region ===我的文章====

                IsShowMyArticle = GetBooleanValue(articleIndexViewModel?.ShowMyArticle, "IsShowMyArticle");
                TempData["IsShowMyArticle"] = IsShowMyArticle;

                if (IsShowMyArticle)
                {
                    articles = articles.Where(s => s.UserID == UserID);
                }
                #endregion

                #region ===篩選資料===
                filter();
                #endregion


                IsCollect = articles.Select(a => queryCollectSQL.Select(c => c.Article).ToList().Contains(a)).ToList();


                #region ===將資料push到viewmodel===

                return new ArticleIndexViewModel
                {
                    userName = userManage.UserName,
                    isUser = true,
                    IsShowCollect = IsShowCollect,
                    IsShowMyArticle= IsShowMyArticle,
                    articles = articles.ToPagedList(pageNumber, pageSize),
                    IsCollect = IsCollect.ToPagedList(pageNumber, pageSize),
                    Page = pageNumber,
                    ShowSearch = searchString

                };
                #endregion
            }

            //篩選
            void filter()
            {
                #region ===分類功能===
                ClassificationString = SelectConvertTemp(articleIndexViewModel.Classification, "Classification");
                if (!String.IsNullOrEmpty(ClassificationString) && ClassificationString != "ALL")
                {
                    articles = articles.Where(s => s.Classification.Contains(ClassificationString));
                }
                #endregion

                #region ===排序功能===
                OrderString = SelectConvertTemp(articleIndexViewModel.Order, "Order");
                if (!String.IsNullOrEmpty(OrderString) && OrderString != "Default")
                {
                    switch (OrderString)
                    {
                        case "時間":
                            articles = articles.OrderBy(s => s.Time);
                            break;
                        case "人氣":
                            articles = articles.OrderByDescending(s => s.Likes);
                            break;
                    }
                }
                #endregion

                #region ===搜尋功能===
                Search();
                #endregion

                #region ===切割文章===
                SplitArticle();
                #endregion
            }

           


            //搜尋
            void Search()
            {
                if (IsShowSearch && articleIndexViewModel.ShowSearch == null)
                {
                    searchString = "";
                }
                else if (articleIndexViewModel.ShowSearch != null)
                {
                    searchString = articleIndexViewModel.ShowSearch;
                }
                else
                {
                    searchString = TempData["ShowSearch"] as string;
                }

                TempData["ShowSearch"] = searchString;

                if (!String.IsNullOrEmpty(searchString))
                {
                    articles = articles.Where(s => s.Title.Contains(searchString)
                                           || s.Content.Contains(searchString));
                }
            }

            //分類:選擇要判斷的變數||要放的Temp
            string SelectConvertTemp(string SelectValue, string TempName)
            {
                string SelectTemp;
                if (!string.IsNullOrEmpty(SelectValue))
                {
                    SelectTemp = SelectValue;
                }
                else if (TempData[TempName] as string != null)
                {
                    SelectTemp = TempData[TempName] as string;
                }
                else
                {
                    SelectTemp = "";
                }

                TempData[TempName] = SelectTemp;

                return SelectTemp;
            }

            void SplitArticle()
            {
                articles = articles.ToList()
                                .Select(article =>
                                {
                                    if (article.Content.Length > 200)
                                    {
                                        article.Content = article.Content.Substring(0, 200) + ".............顯示更多";
                                    }
                                    return article;
                                })
                                .ToList();
            }
        }
        private bool GetBooleanValue(string value, string tempDataKey)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return bool.Parse(value);
            }
            else if (TempData[tempDataKey] is bool)
            {
                return (bool)TempData[tempDataKey];
            }
            else
            {
                return false;
            }
        }
        #endregion
        // -----------------------------------------===============================

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
    }
}
