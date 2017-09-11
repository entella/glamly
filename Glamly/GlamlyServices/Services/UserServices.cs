using GlamlyData;
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
                if (objfb!=null)
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

       public wp_users validationUser(string username, string password)
        {
            try
            {
                       
                using (var context = new GlamlyEntities())
                {

                    return context.wp_users.FirstOrDefault(x => x.user_email == username && x.user_pass == password);                    
                    
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
            return _context.wp_usermeta.Where(m => m.meta_key == "wp_capabilities" && m.meta_value.Contains("administrator")&& m.user_id == id).FirstOrDefault();
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
            return _context.wp_glamly_servicesbookings.ToList();
        }


        public wp_glamly_servicesbookings GetBookingById(int id)
        {
            return _context.wp_glamly_servicesbookings.SingleOrDefault(x => x.id == id);
        }

        public List<wp_glamly_servicesbookings> GetBookingByStatus(string status)
        {
            try
            {
                return _context.wp_glamly_servicesbookings.Where(x => x.status == status).ToList();

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

        public bool IsFacebookLogin(int userid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool isfacebook = false;
            var usermetadata = GetUserMetadatakeybyId(userid);
            var desearlize = serialize.Deserialize(usermetadata.meta_value);
            if (desearlize != null)
            {
                UserData usercollection = serializer.Deserialize<UserData>(Convert.ToString(desearlize));
                var fbid = usercollection.user_facebookid;
                if (!string.IsNullOrEmpty(fbid))               
                    isfacebook = true;                             
            }
            return isfacebook;
        }

        #endregion




    }
}