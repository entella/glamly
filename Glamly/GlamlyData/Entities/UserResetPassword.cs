using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyData.Entities
{
    public class UserResetPassword
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserKey { get; set; }
        public DateTime RequestTime { get; set; }
    }
}