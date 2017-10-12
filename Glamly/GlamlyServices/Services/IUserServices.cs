using GlamlyData;
using GlamlyData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyServices.Services
{
    public interface IUserServices
    {
        List<wp_users> GetUser();

        wp_users GetUser(int id);

        int updateuserdata(wp_usermeta user);

        List<wp_usermeta> GetStylist();

        wp_usermeta GetStylist(int id);

        List<wp_usermeta> GetUserDetail();

        wp_usermeta GetUserDetailById(int id);

        List<wp_glamly_servicestypes> GetServiceTypes();

        wp_glamly_servicestypes GetServiceTypeById(int id);

        List<wp_glamly_servicestypes> GetTypesByServiceId(int id);

        List<wp_glamly_services> GetServices();

        List<wp_glamly_services> GetServiceList();

        List<wp_glamly_services> GetServicesTypes(int id);

        List<wp_glamly_servicesbookings> GetBookings();
  
        List<wp_glamly_servicesbookings> GetBookingByStatus(int userid,string status);            

        wp_glamly_services GetServicesById(int id);

        wp_glamly_servicesbookings GetBookingById(int id);

        List<wp_glamly_servicesbookings> GetBookingByUserId(int id);

        wp_users validationUser(string username);
       
        
        int Saveuserdata(wp_users logindata);

        int Saveusermedadata(wp_usermeta logindata);

        wp_users GetUserByEmailId(string emailId);

        wp_users validationFacebookUser(string fbid);

        wp_usermeta GetUserMetadatakeybyId(decimal userid);

        List<wp_usermeta> GetUserMetadatabyId(decimal userid);

        bool IsFacebookLogin(int userid,string facebookid);

        string savebookingdata(wp_glamly_servicesbookings bookings);

        int updatebookingdata(wp_glamly_servicesbookings bookings);

        int savepaymentdata(wp_glamly_payment payment);      

        List<wp_glamly_payment> GetPaymentList();

        List<wp_glamly_payment> GetPaymentById(int id);

        bool DeleteBooking(string userid);

        bool DeletePaymentRecipt(string bookingid);


        bool DeleteServiceType(int Serviceid);

        int updateservicetypes(wp_glamly_servicestypes types);

        int SaveServicetype(wp_glamly_servicestypes types);

        bool ApprovedBookingByAdmin(string bookingid);

        int GetIdByName(string serviceName);

        List<wp_glamly_servicesbookings> GetApprovedBookingByAdmin(int stylistid);

        List<wp_glamly_servicesbookings> GetApprovedBookingByPro(int stylistid);

        string AcceptBookingByPro(string bookingid);

        string AcceptedBookingByUser(string bookingid);

        int GetBookingCountByUserId(int userid);

        int GetBookingCountByStylistId(int userid);

        int Updateotherservices(string bookingid, string otherservices);

        int SaveEditBookingPaymentRecipt(wp_glamly_payment payment);

        string GetUnderlagdata(string bookingid);

        List<wp_glamly_servicesbookings> GetBookingIdByPro(int stylistid);

        wp_glamly_payment GetUnderlagPaymentdata(string bookingid);

        bool DeletePaymentReceiptById(int paymentid);

        bool SetUserResetPassword(UserResetPassword userResetPassword);

        bool DeleteUserResetPasswordByUserId(int userId);

    }
}