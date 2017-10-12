
using GlamlyData.Entities;
using GlamlyServices.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailHelper
    {


        #region Templates

        JObject arrayLanguagefile;
        string AppLanguagePreferences = "en-US";
        private IUserServices _userService = new UserServices();
        /// <summary>
        /// 
        /// </summary>
        //public EmailHelper()
        //{
        //    using (StreamReader r = new StreamReader(HttpContext.Current.Server.MapPath("~/Scripts\\Resources\\emails.json")))
        //    {
        //        string json = r.ReadToEnd();
        //        arrayLanguagefile = JObject.Parse(json);
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useremail"></param>
        /// <param name="password"></param>
        /// <param name="username"></param>
        public void SendEmailWithTemplatedAddStylistUser(string useremail, string password,string username)
        {
            //  var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//AddUser.html")))
            {
                body = reader.ReadToEnd();
            }
            //Replace the static text
            body = body.Replace("{UserEmail}", useremail);
            body = body.Replace("{Password}", password);
            body = body.Replace("{name}", username);
            System.Text.StringBuilder returnList = new System.Text.StringBuilder();

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, "CustomerServiceEmail".GetValueFromWebConfig(), "Welcome to the Glamly World!", body);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subject"></param>
        public void SendEmailWithTemplateResetPasswordUser(int userId, string subject)
        {
            var userKey = GetUserKeyForPasswordByUserId(userId);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~//Views//Templates//ResetPassword.html")))
            {
                body = reader.ReadToEnd();
            }

            string domainUrl = "DomainUrl".GetValueFromWebConfig();
            domainUrl = domainUrl + "#/ResetPassword?id=" + userKey;

           // subject = getTranslatedValuebyKey(subject);

            //Replace the static text
            body = body.Replace("{RESET_PASSWORD_MAIL_SALUTATION}","Hi");
            body = body.Replace("{RESET_PASSWORD_MAIL_BODY_TEXT1}", "Click the link below to reset your password at Glamly");
            body = body.Replace("{RESET_PASSWORD_LINK}", domainUrl);
            body = body.Replace("{RESET_PASSWORD_LINK_TEXT}", "Reset Password");
            body = body.Replace("{RESET_PASSWORD_MAIL_CLOSING}", "Best regards from Glamly");

            System.Text.StringBuilder returnList = new System.Text.StringBuilder();

            //replacing the body in layout html
            body = body.Replace("{body}", body);
            //Subject of email
            string from = "FromEmail".GetValueFromWebConfig();
            SendEmail(from, "CustomerServiceEmail".GetValueFromWebConfig(), subject, body);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
        public string GetUserKeyForPasswordByUserId(int userId)
        {
            UserResetPassword userResetPassword = new UserResetPassword();
            userResetPassword.UserId = userId;
            userResetPassword.UserKey = Guid.NewGuid().ToString("N");
            userResetPassword.RequestTime = DateTime.Now;

            var isSave = _userService.SetUserResetPassword(userResetPassword);
            if (isSave)
                return userResetPassword.UserKey;
            else
                throw new Exception();
        }
        private string getTranslatedValuebyKey(string KeyName)
        {
            string value = "";
            foreach (var item in arrayLanguagefile)
            {
                if (item.Key == AppLanguagePreferences)
                {
                    JObject obj = JObject.Parse(item.Value.ToString());
                    return value = (string)obj[KeyName];
                }
            }
            return value;
        }

        #endregion

        #region private

        private static bool SendEmail(string from, string to, string subject, string body)
        {
            string TempToEmail = "TempToEmail".GetValueFromWebConfig();
            if (!string.IsNullOrWhiteSpace(TempToEmail))
                to = TempToEmail;

            try
            {
                var mm = new MailMessage(from, to) { Subject = subject, Body = body };

                if (mm.Body != "")
                {
                    mm.IsBodyHtml = true;
                    var smtp = new SmtpClient();
                    smtp.Send(mm);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }   

        #endregion
    }
}