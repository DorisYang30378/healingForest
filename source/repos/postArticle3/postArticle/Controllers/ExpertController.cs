using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace postArticle.Controllers
{
    public class ExpertController : Controller
    {
        public healingForestEntities3    db = new healingForestEntities3();
        public bool CheckLoggedIn() => Session["UserID"] != null;
        // GET: Expert
        public ActionResult Index()
        {
            var expertApplies = db.ExpertApply.Include(e => e.UserManage).ToList(); // 将查询结果转换为列表
            return View(expertApplies); // 将列表传递给视图
        }
        // GET: ExpertApplies/Create
        public ActionResult Apply()
        {
            ViewBag.UserID = new SelectList(db.UserManage, "UserID", "UserName");

            return View();
        }
        private bool IsImageFile(HttpPostedFileBase file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

            return allowedExtensions.Contains(fileExtension.ToLower());
        }

        // POST: ExpertApplies/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(HttpPostedFileBase file, ExpertApply model, [Bind(Include = "ExpertApplyID,ExpertField,ExpertInfo,Status,UserID,ExpertImgURL,Remark")] ExpertApply expertApply)
        {
            if (CheckLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if (IsImageFile(file))
                        {
                            // 獲取檔案名稱
                            string fileName = file.FileName;

                            model.UserID = (int)Session["UserID"];
                            // 將檔案名稱存入模型物件的對應欄位
                            model.ExpertImgURL = fileName;
                            string filePath = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                            file.SaveAs(filePath);


                            // 使用資料庫存取技術將模型物件保存到資料庫中

                            using (healingForestEntities3    dbContext = new healingForestEntities3 ())
                            {
                                dbContext.ExpertApply.Add(model);
                                UserManage user = dbContext.UserManage.FirstOrDefault(u => u.UserID == model.UserID);
                                if (user != null)
                                {
                                    // 修改会员数据以反映成为专家的状态或其他信息
                                    model.UserManage.UserType ="Expert"; // 假设有一个表示成为专家状态的布尔字段
                                                          // 其他可能的修改...
                                }
                                dbContext.SaveChanges();
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Message = "請上傳有效的圖片檔案";
                        }

                    }
                }



            }

            ViewBag.UserID = new SelectList(db.UserManage, "UserID", "UserName", expertApply.UserID);
            return View(expertApply);
        }
    }
}