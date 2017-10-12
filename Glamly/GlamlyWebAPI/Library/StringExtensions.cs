using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromWebConfig(this string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
    }
}