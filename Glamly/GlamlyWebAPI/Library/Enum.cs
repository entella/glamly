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
        Active = 0,
        Blocked = 1
    }
    /// <summary>
    /// Enum define the user type
    /// </summary>
    public enum UserTypeEnum
    {
        Admin = 1,
        Stylist = 2,
        Person = 3,
    }

}