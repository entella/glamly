using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyData.Entities
{
    public class Servicewithtype
    {
        public string servicename { get; set; }
        public string typename { get; set; }
        public string price { get; set; }
    }

    public class Payment
    {
        public string acquirer { get; set; }
        public string amount { get; set; }
        public string approvalcode { get; set; }
        public string calcfee { get; set; }
        public string cardexpdate { get; set; }
        public string cardnomask { get; set; }
        public string cardprefix { get; set; }
        public string cardtype { get; set; }
        public string currency { get; set; }
        public string dibsInternalIdentifier { get; set; }
        public int fee { get; set; }
        public string fullreply { get; set; }
        public string lang { get; set; }
        public string merchant { get; set; }
        public int merchantid { get; set; }
        public string method { get; set; }
        public string mobilelib { get; set; }
        public string orderid { get; set; }
        public string paytype { get; set; }
        public string platform { get; set; }
        public string status { get; set; }
        public string test { get; set; }
        public string textreply { get; set; }
        public string theme { get; set; }
        public string timeout { get; set; }
        public string transact { get; set; }
        public string version { get; set; }
        public int userid { get; set; }
        public int bookingid { get; set; }
    }

    public class Booking
    {
        public int Id { get; set; }
        public string bookingid { get; set; }
        public string service { get; set; }
        public string type { get; set; }
        public string datetime { get; set; }
        public string altdatetime { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string personal { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string billingaddress { get; set; }
        public string message { get; set; }
        public string newsletter { get; set; }
        public string status { get; set; }
        public int userid { get; set; }
        public string isedit { get; set; } 
        public List<Servicewithtype> servicewithtypes { get; set; }
        public Payment payment { get; set; }
    }
}