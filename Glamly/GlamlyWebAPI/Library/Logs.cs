using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


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
        #endregion
    }
}