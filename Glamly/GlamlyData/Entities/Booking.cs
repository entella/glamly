﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyData.Entities
{
    public class ServiceWithType
    {
        public string servicename { get; set; }
        public string typename { get; set; }
        public string price { get; set; }
    }
  
    public class updatebooking
    {
        public string bookingid { get; set; }
        public List<otherservices> otherservices { get; set; }
    }


    public class otherservices
    {      
        public string typename { get; set; }
        public string price { get; set; }
    }

    public class AvailsDates
    {
        public int stylistid { get; set; }
        public List<StylistDates> dates { get; set; }
    }


    public class StylistDates
    {
        public int id { get; set; }
        public int stylistid { get; set; }
        public string date { get; set; }
        public string status { get; set; }
        public string isdeleted { get; set; }
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
        public string bookingid { get; set; }
        public string servicewithtypes { get; set; }   
        public string paymentdate { get; set; }
        public string isdeleted { get; set; }
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
        public List<ServiceWithType> servicewithtypes { get; set; }
        public Payment payment { get; set; }
        public string otherservices { get; set; }
        public string isdeleted { get; set; }
        public string workflowstatus { get; set; }
        public int stylistId { get; set; }
        public string comments { get; set; }
    }

    public class PaymentReceipt
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
        public string bookingid { get; set; }
        public List<ServiceWithType> servicewithtypes { get; set; }
        public List<otherservices> otherservices { get; set; }
        public string paymentdate { get; set; }
        public string isdeleted { get; set; }
    }

    public class UnderLag
    {
        public int BookingsCount { get; set; }
        public int TotalIncome { get; set; }
        public int StatisticsCount { get; set; }
    }
}