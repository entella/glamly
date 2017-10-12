using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlamlyData.Entities
{
    public class ChangePassword
    {
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}