using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace _Platform.Service
{
    public class MaillService
    {
        public string from = System.Web.Configuration.WebConfigurationManager.AppSettings["MailHost"];

        public Boolean EnableSSL = string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["MailEnableSsl"])
            ? false : Boolean.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["MailEnableSsl"].ToString());
        public int SmtpPort = string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["MailPort"])
            ? 25 : int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["MailPort"]);
        public string MailMailFormAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["MailMailFromAddress"];

        public string MailUserName = System.Web.Configuration.WebConfigurationManager.AppSettings["MailUserName"];
        public string MailPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];
        public Boolean EnabelNetworkCredential = true;


        public bool SendMail(string to, string mailSubject, string mailBody)
        {
            string errorMsg = "";
            bool ret = false;
            #region 取得寄信相關設定

            //是否開啟測試MAIL
            bool IsEmailTest = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["IsEmailTest"]);
            //測試MAIL string
            string TestEmail = string.Empty;
            #endregion
            #region 寄送信件
            try
            {
                #region ==收寄件資料處理==
                MailMessage mail = new MailMessage();
                //寄件者
                mail.From = new MailAddress(MailMailFormAddress, "療癒之森");
                //收件者(To)
                if (IsEmailTest)
                {
                    to = TestEmail;
                }
                if (!string.IsNullOrEmpty(to))
                {
                    string[] strMail = to.Split(';');
                    for (int i = 0; i < strMail.Length; i++)
                    {
                        mail.To.Add(strMail[i]);
                    }
                }
                #endregion
                #region==文本處裡==
                //標題
                mail.Subject = mailSubject;
                //內文
                mail.IsBodyHtml = true;
                mail.Body = mailBody.Trim();
                #endregion

                //SMTP Server
                SmtpClient smtp = new SmtpClient(from);
                smtp.Port = SmtpPort;
                smtp.EnableSsl = EnableSSL;
                if (EnabelNetworkCredential)
                {
                    NetworkCredential cred = new NetworkCredential(MailUserName, MailPassword);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = cred;
                }

                smtp.Send(mail);
            }
            catch (Exception err)
            {
                errorMsg = err.Message;
            }
            #endregion
            return ret;
        }

    }
}