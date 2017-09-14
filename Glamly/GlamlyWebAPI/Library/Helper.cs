using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// Helper class provides the common functions for the API
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        private static Random random = new Random();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //public string UserToken
        //{
        //    get
        //    {
        //        string userAutorization = Convert.ToString(HttpContext.Current.Request.Headers["X-User-Authorization"]);
        //        if (!string.IsNullOrWhiteSpace(userAutorization))
        //            return userAutorization;
        //        else
        //            return string.Empty;
        //    }
        //}

        //public string AppConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["AppConnectionString"]?.ToString());
        //public bool IsApiAuthorized
        //{
        //    get
        //    {
        //        bool success = false;
        //        try
        //        {
        //            //Set success
        //            success = (HttpContext.Current.Request.Headers["X-API-Authorization"] == WebConfigurationManager.AppSettings["APIKey"]);
        //            if (!success)
        //                Logs.Add(string.Format("IsApiAuthorized :: APIKey is not authorized."), "Cab");
        //        }
        //        catch (Exception Ex)
        //        {
        //            Logs.Add(string.Format("{0} Method: {1} Error: {2}", DateTime.Now, MethodBase.GetCurrentMethod().Name, Ex.ToString()), "Cab");
        //        }
        //        return success;
        //    }
        //}


        /// <summary>
        /// Method to get the password with salt
        /// </summary>
        /// <returns></returns>
        public static string getPasswordSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int maxLength = 8;

            // Empty salt array
            byte[] byteSalt = new byte[maxLength];

            // Build the random bytes
            random.GetNonZeroBytes(byteSalt);

            // Return the string encoded salt
            return Convert.ToBase64String(byteSalt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToValidate"></param>
        /// <returns></returns>
        public static bool IsEmail(string stringToValidate)
        {
            try
            {
                string pattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

                return Regex.IsMatch(stringToValidate, pattern);
            }
            catch (Exception)
            {

            }

            return false;
        }


    }
}