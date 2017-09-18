using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace GlamlyWebAPI.Library
{
     /// <summary>
    /// Class creates the logs for request and response
    /// </summary>
    public class Logs
    {
        #region Variables
        /// <summary>
        /// variable to show the log message
        /// </summary>
        public static string message;
        #endregion

        #region Public methods
        /// <summary>
        /// Add method log the every request and response of the API
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fileName"></param>
        /// 

        public static void Add(string message, string fileName = "")
        {
            StreamWriter streamWriter = null;

            try
            {
                // Open file and add message
                string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Log/{0}_{1}.log", !string.IsNullOrWhiteSpace(fileName) ? fileName : "Glamly", DateTime.Now.ToString("yyyy-MM-dd")));
                streamWriter = new StreamWriter(filePath, true);
                streamWriter.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
            }
            catch (Exception ex)
            {
                Logs.message = ex.Message;
            }
            finally
            {
                if (streamWriter != null)
                {
                    // Close file
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }


        /// <summary>
        /// To send error on email
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="shortDescription">Short Description</param>
        public static void MailError(Exception ex, string shortDescription = "")
        {
            string MailTo = ConfigurationManager.AppSettings["ErrorsEmail"];
            string MailFrom = ConfigurationManager.AppSettings["FromEmail"];

            if (WebConfigurationManager.AppSettings["SendEmail"] != null ? Convert.ToBoolean(WebConfigurationManager.AppSettings["SendEmail"]) : false)
            {
                if (!string.IsNullOrWhiteSpace(MailTo) && !string.IsNullOrWhiteSpace(MailFrom))
                {
                    MailMessage mailMessage = new MailMessage();

                    try
                    {
                        mailMessage.To.Add(MailTo);
                        mailMessage.From = new MailAddress(MailFrom);
                        string host = HttpContext.Current.Request.Url.Host;
                        mailMessage.Subject = $"Subject: Caught error from api server {host}";
                        mailMessage.Body = (!string.IsNullOrWhiteSpace(shortDescription) ? shortDescription + Environment.NewLine + Environment.NewLine : "") + ex.ToString();

                        using (SmtpClient smtpClient = new SmtpClient())
                        {
                            smtpClient.UseDefaultCredentials = true;
                            smtpClient.EnableSsl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSsl"]) ? Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]) : true;
                            smtpClient.Host = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Host"]) ? ConfigurationManager.AppSettings["Host"] : "smtp.gmail.com";
                            smtpClient.Port = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Port"]) ? Convert.ToInt32(ConfigurationManager.AppSettings["Port"]) : 587;

                            NetworkCredential networkCredential = new NetworkCredential();

                            if (ConfigurationManager.AppSettings["UserName"] != null && ConfigurationManager.AppSettings["UserName"] != null)
                            {
                                networkCredential.UserName = ConfigurationManager.AppSettings["UserName"];
                                networkCredential.Password = ConfigurationManager.AppSettings["Password"];

                                smtpClient.Credentials = networkCredential;
                            }

                            smtpClient.Send(mailMessage);
                        }
                    }
                    catch  //Do not log exeption heare cause it cause infinite loop
                    {

                    }
                }
            }
        }
        #endregion
    }
}