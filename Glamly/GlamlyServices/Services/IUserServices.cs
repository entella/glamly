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

        List<wp_usermeta> GetStylist();

        wp_usermeta GetStylist(int id);

        List<wp_usermeta> GetUserDetail();

        wp_usermeta GetUserDetailById(int id);

        wp_glamly_servicestypes GetServiceTypeById(int id);

        List<wp_glamly_servicestypes> GetTypesByServiceId(int id);

        List<wp_glamly_services> GetServices();

        List<wp_glamly_services> GetServicesTypes(int id);

        List<wp_glamly_servicesbookings> GetBookings();
  
        List<wp_glamly_servicesbookings> GetBookingByStatus(string status);            

        wp_glamly_services GetServicesById(int id);

        wp_glamly_servicesbookings GetBookingById(int id);

        wp_users validationUser(string username, string password);
       
        
        int Saveuserdata(wp_users logindata);

        int Saveusermedadata(wp_usermeta logindata);

        wp_users GetUserByEmailId(string emailId);

        wp_users validationFacebookUser(string fbid);

        wp_usermeta GetUserMetadatakeybyId(decimal userid);

        List<wp_usermeta> GetUserMetadatabyId(decimal userid);

        bool IsFacebookLogin(int userid,string facebookid);

        int savebookingdata(wp_glamly_servicesbookings bookings);

        int savepaymentdata(wp_glamly_payment payment);
    }
}