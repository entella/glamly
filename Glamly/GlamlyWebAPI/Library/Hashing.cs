using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// Hashing class is help to implement the MD5.
    /// </summary>
    public class Hashing
    {
        /// <summary>
        /// MD5 Hashing with salt
        /// </summary>
        /// <param name="dataToHash"></param>
        /// <param name="saltKey"></param>
        /// <returns></returns>
        public static string MD5Hash(string dataToHash, string saltKey)
        {
            HMACMD5 objHMACMD5 = null;
            Byte[] byteSalt = System.Text.Encoding.UTF8.GetBytes(saltKey);
            Byte[] bytePassword = System.Text.Encoding.UTF8.GetBytes(dataToHash);

            if (byteSalt.Length > 0)
                objHMACMD5 = new HMACMD5(byteSalt);
            else
                objHMACMD5 = new HMACMD5();

            if (bytePassword.Length > 0)
                objHMACMD5.ComputeHash(bytePassword);

            return Convert.ToBase64String(objHMACMD5.Hash);
        }
    }
}