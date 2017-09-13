using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// Enum define the user status
    /// </summary>
    public enum UserStatusEnum
    {
        /// <summary>
        /// 
        /// </summary>
        Active = 0,
        /// <summary>
        /// 
        /// </summary>
        Blocked = 1
    }
    /// <summary>
    /// Enum define the user type
    /// </summary>
    public enum UserTypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        Admin = 1,
        /// <summary>
        /// 
        /// </summary>
        Stylist = 2,
        /// <summary>
        /// 
        /// </summary>
        Person = 3,
    }
    /// <summary>
    /// Enum define booking status
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Draft = 0,
        
      /// <summary>
      /// 
      /// </summary>
        Approved = 21,
        /// <summary>
        /// 
        /// </summary>
        Rejected = 31,
        /// <summary>
        /// 
        /// </summary>
        InReporting = 41,
        /// <summary>
        /// 
        /// </summary>        
        Completed = 91
    }


}