using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyData.Entities
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
        ApprovedByAdmin = 11,
        /// <summary>
        /// 
        /// </summary>
        ApprovedByProUser = 21,
        /// <summary>
        /// 
        /// </summary>
        AcceptBookingByUser = 31,
        /// <summary>
        /// 
        /// </summary>
        RejectedByProUser = 41,
        /// <summary>
        /// 
        /// </summary>
        RejectedByCustomer = 51,
        /// <summary>
        /// 
        /// </summary>
        InEditing = 61,
        /// <summary>
        /// 
        /// </summary>
        CancelByAmin = 71,
        /// <summary>
        /// 
        /// </summary>   
        /// 
        Completed = 91,
        /// <summary>
        /// 
        /// </summary>
        InWorkFlowWithCustomer = 01,
        /// <summary>
        /// 
        /// </summary>
        InWorkFlowWithProUser = 02,
        /// <summary>
        /// 
        /// </summary>
        InWorkFlowWithAdmin = 03,
    }

}