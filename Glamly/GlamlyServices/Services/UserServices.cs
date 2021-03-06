﻿using GlamlyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamlyData.Entities;
using Conversive.PHPSerializationLibrary;
using System.Web.Script.Serialization;

namespace GlamlyServices.Services
{
    public class UserServices : IUserServices
    {
        //private readonly GlamlyEntities _context;

        //public UserServices(GlamlyEntities context)
        //{
        //    _context = context;
        //}

        private GlamlyEntities _context = new GlamlyEntities();
        Serializer serialize = new Serializer();

        #region "User Details"

        public List<wp_users> GetUser()
        {
            return _context.wp_users.ToList();
        }

        public wp_users GetUser(int id)
        {
            return _context.wp_users.Where(x => x.ID == id).FirstOrDefault();
        }

        public List<wp_usermeta> GetUserDetail()
        {
            return _context.wp_usermeta.ToList();
        }

        public wp_usermeta GetUserDetailById(int id)
        {
            return _context.wp_usermeta.Where(x => x.user_id == id).FirstOrDefault();
        }

        public wp_users GetUserByEmailId(string emailId)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_users.FirstOrDefault(x => x.user_email == emailId);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public wp_usermeta GetUserMetadatakeybyId(decimal userid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_usermeta.FirstOrDefault(ud => ud.user_id == userid);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public wp_users validationFacebookUser(string fbid)
        {
            try
            {
                wp_usermeta objfb = new wp_usermeta();
                wp_users obj = new wp_users();
                objfb = _context.wp_usermeta.FirstOrDefault(ud => ud.meta_key == "facebook_id" && ud.meta_value.Contains(fbid));
                if (objfb != null)
                {
                    obj.ID = objfb.user_id;
                }
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public wp_users validationUser(string username)
        {
            try
            {

                using (var context = new GlamlyEntities())
                {

                    return context.wp_users.FirstOrDefault(x => x.user_email == username);

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public int Saveuserdata(wp_users userdata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(userdata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(userdata.ID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public int Saveusermedadata(wp_usermeta usermetadata)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(usermetadata).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return Convert.ToInt32(usermetadata.user_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public List<wp_usermeta> GetStylist()
        {
            return _context.wp_usermeta.Where(m => m.meta_key == "wp_capabilities" && m.meta_value.Contains("administrator")).ToList();
        }

        public wp_usermeta GetStylist(int id)
        {
            return _context.wp_usermeta.Where(m => m.meta_key == "wp_capabilities" && m.meta_value.Contains("administrator") && m.user_id == id).FirstOrDefault();
        }

        #endregion

        #region "Service Details"

        public List<wp_glamly_services> GetServices()
        {
            return _context.wp_glamly_services.ToList();
        }

        public wp_glamly_services GetServicesById(int id)
        {
            return _context.wp_glamly_services.SingleOrDefault(x => x.id == id);
        }


        #endregion

        #region "Booking Details"

        public List<wp_glamly_servicesbookings> GetBookings()
        {
            return _context.wp_glamly_servicesbookings.Where(b => b.isdeleted == "false" && !string.IsNullOrEmpty(b.servicewithtypes)).ToList();
        }


        public wp_glamly_servicesbookings GetBookingById(int id)
        {
            return _context.wp_glamly_servicesbookings.SingleOrDefault(x => x.id == id && x.isdeleted == "false");
        }

        public List<wp_glamly_servicesbookings> GetBookingByStatus(int userid,string status)
        {
            try
            {
                return _context.wp_glamly_servicesbookings.Where(x => x.status == status && x.userid == userid).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public wp_glamly_servicestypes GetServiceTypeById(int id)
        {
            return _context.wp_glamly_servicestypes.SingleOrDefault(x => x.id == id);
        }

        public List<wp_glamly_servicestypes> GetTypesByServiceId(int id)
        {
            return _context.wp_glamly_servicestypes.Where(x => x.serviceid == id).ToList();
        }

        public List<wp_usermeta> GetUserMetadatabyId(decimal userid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    return context.wp_usermeta.Where(ud => ud.user_id == userid).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<wp_glamly_services> GetServicesTypes(int id)
        {
            return _context.wp_glamly_services.Include("wp_glamly_servicestypes").Where(s => s.id == id).ToList();
        }

        public bool IsFacebookLogin(int userid, string facebookid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool isfacebook = false;
            var usermetadata = GetUserMetadatakeybyId(userid);
            var desearlize = serialize.Deserialize(usermetadata.meta_value);
            if (desearlize != null)
            {
                UserData usercollection = serializer.Deserialize<UserData>(Convert.ToString(desearlize));
                var fbid = usercollection.user_facebookid;
                if (!string.IsNullOrEmpty(fbid) && fbid.Trim().ToLower() == facebookid.Trim().ToLower())
                    isfacebook = true;
            }
            return isfacebook;
        }

        public string savebookingdata(wp_glamly_servicesbookings bookings)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(bookings).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return bookings.bookingid;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public int savepaymentdata(wp_glamly_payment payment)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(payment).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return payment.userid;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public int updatebookingdata(wp_glamly_servicesbookings bookings)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(bookings).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return bookings.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("booking not updated");
            }
        }       

        public List<wp_glamly_payment> GetPaymentList()
        {
            return _context.wp_glamly_payment.Where(b => b.isdeleted == "false").ToList();
        }

        public List<wp_glamly_payment> GetPaymentById(int id)
        {
            return _context.wp_glamly_payment.Where(x => x.userid == id &&  x.isdeleted == "false").ToList();
        }

        public List<wp_glamly_servicesbookings> GetBookingByUserId(int id)
        {
            return _context.wp_glamly_servicesbookings.Where(x => x.userid == id && x.isdeleted == "false").ToList();
        }

        public int updateuserdata(wp_usermeta user)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(user.user_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User data not updated");
            }
        }

        public bool DeleteBooking(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToDelete.isdeleted = "true";
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeletePaymentRecipt(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_payment.FirstOrDefault(x => x.bookingid == bookingid);
                    userToDelete.isdeleted = "true";
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<wp_glamly_services> GetServiceList()
        {
            return _context.wp_glamly_services.ToList();
        }

        public List<wp_glamly_servicestypes> GetServiceTypes()
        {
            return _context.wp_glamly_servicestypes.ToList();
        }
        public int updateservicetypes(wp_glamly_servicestypes types)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(types).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return Convert.ToInt32(types.id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User data not updated");
            }
        }

        public int SaveServicetype(wp_glamly_servicestypes types)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(types).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return types.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool DeleteServiceType(int Serviceid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicestypes.FirstOrDefault(x => x.id == Serviceid);
                    userToDelete.status = 0;
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ApprovedBookingByAdmin(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToDelete.workflowstatus = (int)BookingStatus.ApprovedByAdmin;
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetIdByName(string serviceName)
        {
            return _context.wp_glamly_services.Where(x => x.servicename == serviceName.Trim()).FirstOrDefault().id;
        }

        public List<wp_glamly_servicesbookings> GetApprovedBookingByAdmin(int stylistid)
        {
            return _context.wp_glamly_servicesbookings.Where(x => x.workflowstatus == 11 && x.stylistid== stylistid).ToList();
        }


        public List<wp_glamly_servicesbookings> GetApprovedBookingByPro(int stylistid)
        {
            return _context.wp_glamly_servicesbookings.Where(x => x.stylistid == stylistid && x.workflowstatus == 21).ToList();
        }


        public string AcceptBookingByPro(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdate = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToUpdate.workflowstatus = (int)BookingStatus.ApprovedByProUser;                    
                    context.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return bookingid;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AcceptedBookingByUser(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdate = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToUpdate.workflowstatus = (int)BookingStatus.AcceptBookingByUser;
                    userToUpdate.isedit = "false";
                    context.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return bookingid;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public int GetBookingCountByUserId(int userid)
        {
            return _context.wp_glamly_servicesbookings.Count(x => x.userid == userid && x.isdeleted=="false");
        }    

        public int Updateotherservices(string bookingid, string otherservices)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToUpdate = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToUpdate.otherservices = otherservices;
                    userToUpdate.isedit = "true";
                    context.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return userToUpdate.id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int SaveEditBookingPaymentRecipt(wp_glamly_payment payment)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    context.Entry(payment).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return payment.id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetBookingCountByStylistId(int stylistid)
        {
            return _context.wp_glamly_servicesbookings.Count(x => x.stylistid == stylistid && x.isdeleted == "false");
        }

        public string GetUnderlagdata(string bookingid)
        {
            return _context.wp_glamly_payment.Where(x => x.bookingid == bookingid).FirstOrDefault().amount;           
        }

        public wp_glamly_payment GetUnderlagPaymentdata(string bookingid)
        {
            return _context.wp_glamly_payment.FirstOrDefault(x => x.bookingid == bookingid);            
        }

        public List<wp_glamly_servicesbookings> GetBookingIdByPro(int stylistid)
        {
            return _context.wp_glamly_servicesbookings.Where(x => x.stylistid == stylistid && x.workflowstatus == 31).ToList();
        }


        public bool DeletePaymentReceiptById(int paymentid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_payment.FirstOrDefault(x => x.id == paymentid);
                    userToDelete.isdeleted = "true";
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetUserResetPassword(UserResetPassword userResetPassword)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userResetPasswordObjectDB = context.wp_glamly_userresetpassword.FirstOrDefault(x => x.userid == userResetPassword.UserId);
                    if (userResetPasswordObjectDB != null)
                        context.Entry(userResetPasswordObjectDB).State = System.Data.Entity.EntityState.Deleted;
                    wp_glamly_userresetpassword obj = new wp_glamly_userresetpassword();
                    obj.userid = userResetPassword.UserId;
                    obj.userkey = userResetPassword.UserKey;
                    obj.requesttime = userResetPassword.RequestTime;
                    context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool DeleteUserResetPasswordByUserId(int userId)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userResetPasswordToDelete = context.wp_glamly_userresetpassword.FirstOrDefault(x => x.userid == userId);
                    context.Entry(userResetPasswordToDelete).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool DeclineBookingByProUser(string bookingid)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    var userToDelete = context.wp_glamly_servicesbookings.FirstOrDefault(x => x.bookingid == bookingid);
                    userToDelete.workflowstatus = (int)BookingStatus.RejectedByProUser;
                    context.Entry(userToDelete).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int SaveStylistAvailsDates( List<StylistDates> dates)
        {
            try
            {
                List<wp_glamly_stylistschedules> dateobj = new List<wp_glamly_stylistschedules>();
                using (var context = new GlamlyEntities())
                {
                    wp_glamly_stylistschedules objadd = new wp_glamly_stylistschedules();

                    foreach (var item in dates)
                    {

                        objadd.date =Convert.ToDateTime(item.date);
                        objadd.isdeleted = item.isdeleted;
                        objadd.stylistId = item.stylistid;
                        objadd.status = item.status;
                        context.Entry(objadd).State = System.Data.Entity.EntityState.Added;                       
                        context.SaveChanges();
                    }
                  

                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public int DeleteStylistAvailsDates(List<StylistDates> dates)
        {
            try
            {
                using (var context = new GlamlyEntities())
                {
                    wp_glamly_stylistschedules objdelete = new wp_glamly_stylistschedules();
                    foreach (var item in dates)
                    {

                        var deleteObjectDB = context.wp_glamly_stylistschedules.FirstOrDefault(x => x.id == item.id);
                        if (deleteObjectDB != null)
                            context.Entry(deleteObjectDB).State = System.Data.Entity.EntityState.Deleted;                                             
                    }                    
                    context.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }

    #endregion
}
