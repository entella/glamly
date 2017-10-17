using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using GlamlyWebAPI.Library;
using GlamlyServices.Services;
using GlamlyData;
using NLog;
using GlamlyData.Entities;
using System.Web.Script.Serialization;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using WebApi.OutputCache.V2;

namespace GlamlyWebAPI.Controllers
{
    /// <summary>
    /// User controller contains the user information
    /// </summary>
    /// 
    [RoutePrefix("api/v1/user")]
    public class UsersController : ApiController
    {
        #region Variables
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IUserServices _userService = new UserServices();
        EmailHelper emailHelper = new EmailHelper();
        Helper helpers = new Helper();
        Serializer serialize = new Serializer();
        JavaScriptSerializer javaserializer = new JavaScriptSerializer();
        #endregion

        #region "User"

        /// <summary>
        /// Get the user list.
        /// </summary>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("allusers"), HttpGet]
        public ResponseExtended<List<wp_users>> GetUser()
        {
            var resp = new ResponseExtended<List<wp_users>>();
            var user = new List<wp_users>();
            try
            {
                user = _userService.GetUser();
                if (user != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = user;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.UserNotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.Add(string.Format("{0} Method: {1} Error: {2}", DateTime.Now, MethodBase.GetCurrentMethod().Name, ex.ToString()), "Users");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Get the user details by id.
        /// </summary>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("userdetail/{id}"), HttpGet]
        public ResponseExtended<UserData> GetUserdetails(int id)
        {
            var resp = new ResponseExtended<UserData>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(id);
                if (usermetadata != null)
                {
                    if (string.IsNullOrEmpty(usermetadata.meta_value))
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                        return resp;
                    }

                    else
                    {
                        var desearlize = serialize.Deserialize(usermetadata.meta_value);
                        UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                        if (usercollection != null)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseData = usercollection;
                            return resp;
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                            resp.ResponseData = usercollection;
                            return resp;
                        }
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "userdetail/id");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Update the user data.
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("updateUser"), HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateUserDetails(UserData LoginModel)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(LoginModel.ID);
                if (usermetadata != null)
                {
                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                    if (usercollection != null)
                    {
                        usercollection.first_name = !string.IsNullOrEmpty(LoginModel.first_name) ? LoginModel.first_name : string.Empty;
                        usercollection.last_name = !string.IsNullOrEmpty(LoginModel.last_name) ? LoginModel.last_name : string.Empty;
                        usercollection.mobile = !string.IsNullOrEmpty(LoginModel.mobile) ? LoginModel.mobile : string.Empty;
                        usercollection.user_email = !string.IsNullOrEmpty(LoginModel.user_email) ? LoginModel.user_email : string.Empty;
                        usercollection.offer = LoginModel.offer;
                        usercollection.upcomingbookings = LoginModel.upcomingbookings;
                        usercollection.notificationall = LoginModel.notificationall;

                        var jsonupdate = JsonConvert.SerializeObject(usercollection);
                        usermetadata.meta_value = serialize.Serialize(jsonupdate);
                        int userid = _userService.updateuserdata(usermetadata);

                        if (userid > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "User has been updated successfully";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updateUser");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Get the stylist pro user list.
        /// </summary>
        /// <returns></returns>
        [Route("prousers"), HttpGet]
        public List<wp_glamly_servicesbookings> GetStylistUser()
        {
            try
            {
                Serializer serialize = new Serializer();
                var stylistlist = new List<wp_glamly_servicesbookings>();
                var servicelist = new List<wp_glamly_services>();
                string serlist = string.Empty;
                string servicetypelist = string.Empty;
                stylistlist = _userService.GetBookings();
                foreach (var item in stylistlist)
                {
                    var stylishservice = serialize.Deserialize(item.service);
                    var stylishtype = serialize.Deserialize(item.type);
                    IEnumerable enumerableservice = stylishservice as IEnumerable;
                    IEnumerable enumerabletype = stylishtype as IEnumerable;
                    if (enumerableservice != null)
                    {
                        foreach (object element in enumerableservice)
                        {
                            wp_glamly_services slist = _userService.GetServicesById(Convert.ToInt32(element));
                            var obj = stylistlist.FirstOrDefault(x => x.id == item.id);
                            if (obj != null)
                                serlist += slist.servicename + ",";
                        }
                        item.service = Convert.ToString(serlist.TrimEnd(','));
                        serlist = string.Empty;
                    }

                    if (enumerabletype != null)
                    {
                        foreach (object element in enumerabletype)
                        {
                            wp_glamly_servicestypes slist1 = _userService.GetServiceTypeById(Convert.ToInt32(element));
                            var obj = stylistlist.FirstOrDefault(x => x.id == item.id);
                            if (obj != null)
                                servicetypelist += slist1.typename + ",";
                        }
                        item.type = Convert.ToString(servicetypelist.TrimEnd(','));
                        servicetypelist = string.Empty;
                    }
                }
                return stylistlist;
            }
            catch (Exception exception)
            {
                //  resp.ResponseCode = Response.Codes.ApiUnauthorized;
                // resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }

        }



        /// <summary>
        /// Login the user with valid credentials
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("login"), HttpPost]
        public ResponseExtended<Dictionary<string, string>> UserLogin(UserData user)
        {
            ResponseExtended<Dictionary<string, string>> resp = new ResponseExtended<Dictionary<string, string>>();
            // string baseAddress = "http://192.168.1.120:8010"; //DevUrl
            // string baseAddress = "http://localhost:51455";    //LocalUrl
            // string baseAddress = "http://glamlyapi.entella.com";//StageUrl

            string baseAddress = ConfigurationManager.AppSettings["StageUrl"];
            Token token = new Token();
            try
            {
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.user_email))
                    {
                        if (string.IsNullOrEmpty(user.user_email) || !Helper.IsEmail(user.user_email.Replace("'", "''")))
                        {
                            resp.ResponseCode = Response.Codes.InvalidEmailAddress;
                            return resp;
                        }
                    }
                    if (string.IsNullOrEmpty(user.user_pass) && string.IsNullOrEmpty(user.user_facebookid))
                    {
                        resp.ResponseCode = Response.Codes.InvalidPassword;
                        return resp;
                    }

                    using (var client = new HttpClient())
                    {
                        var form = new Dictionary<string, string>
                       {
                           {"grant_type", "password"},
                           {"username", user.user_email},
                           {"password", user.user_pass},
                           {"facebookid", user.user_facebookid},
                       };
                        var tokenResponse = client.PostAsync(baseAddress + "/AuthenticationToken", new FormUrlEncodedContent(form)).Result;


                        token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                        if (string.IsNullOrEmpty(token.Error))
                        {
                            Dictionary<string, string> typeObject = new Dictionary<string, string>();
                            typeObject.Add("Token", Convert.ToString(token.AccessToken));
                            typeObject.Add("Userid", Convert.ToString(token.Id));
                            typeObject.Add("FirstName", Convert.ToString(token.FirstName));
                            typeObject.Add("UserEmail", Convert.ToString(user.user_email));
                            typeObject.Add("UserType", Convert.ToString(token.UserType));
                            typeObject.Add("Mobile", Convert.ToString(token.Mobile));
                            resp.ResponseData = typeObject;
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "Token has been created successfully.";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.ApiUnauthorized;
                        }
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "userdetail/id");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        /// <summary>
        /// Register the user 
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns></returns>        
        [Route("register"), HttpPost, AllowAnonymous]
        public ResponseExtended<System.Web.Mvc.JsonResult> RegisterUser(UserData LoginModel)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string userSalt = string.Empty;
            string userPassword = string.Empty;
            string hashedPassword = string.Empty;
            string name = string.Empty;
            string customernumber = string.Empty;
            string userJsonObject = string.Empty;
            try
            {
                userSalt = Helper.getPasswordSalt();

                if (!string.IsNullOrEmpty(LoginModel.user_pass))
                {
                    userPassword = Hashing.MD5Hash(LoginModel.user_pass.Trim(), userSalt);
                }

                //MySqlConnection connectionString = new MySqlConnection("server=52.209.186.41;user id=usrglamly;password=G@Lm!Y;database=wp_glamly;");
                //connectionString.Open();
                //MySqlTransaction myTrans = connectionString.BeginTransaction();

                if (LoginModel != null)
                {

                    int userid = 0; string usertype = string.Empty; string FacebookIdExist = string.Empty; string UserEmailExist = string.Empty; bool IsFacebookIdExist = false; bool IsUserEmailExist = false;

                    var udata = _userService.GetUser().Where(users => users.user_email == LoginModel.user_email).FirstOrDefault();

                    if (udata != null)
                    {
                        userid = Convert.ToInt32(udata.ID);
                        var usermetadata = _userService.GetUserMetadatakeybyId(userid);
                        if (usermetadata != null)
                        {
                            var desearlize = serialize.Deserialize(usermetadata.meta_value);
                            UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));
                            if (usercollection != null)
                            {

                                if (!string.IsNullOrEmpty(LoginModel.user_facebookid) && !string.IsNullOrWhiteSpace(LoginModel.user_facebookid))
                                {
                                    if (LoginModel.user_facebookid == usercollection.user_facebookid)
                                    {
                                        IsFacebookIdExist = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(LoginModel.user_type) && !string.IsNullOrWhiteSpace(LoginModel.user_type) && udata != null)
                                {
                                    IsUserEmailExist = _userService.GetUser().Exists(users => users.user_email == LoginModel.user_email && usercollection.user_type == LoginModel.user_type);

                                }
                            }
                        }
                    }

                    if (IsFacebookIdExist || IsUserEmailExist)
                    {
                        resp.ResponseCode = Response.Codes.Exists;
                        return resp;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(LoginModel.user_facebookid) && !string.IsNullOrWhiteSpace(LoginModel.user_facebookid))
                        {
                            wp_users obj = new wp_users();
                            obj.user_email = LoginModel.user_email;
                            obj.user_login = "";
                            obj.user_nicename = "";
                            obj.user_pass = "";
                            obj.user_registered = DateTime.MinValue;
                            obj.user_status = 0;
                            obj.user_url = "";
                            obj.user_activation_key = "";
                            obj.display_name = "";

                            int user_id = _userService.Saveuserdata(obj);
                            LoginModel.ID = Convert.ToInt32(user_id);
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = LoginModel.user_type == "customer" ? "customerfb_logindata" : "profb_logindata";
                            obj1.meta_value = serialize.Serialize(userJsonObject);
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            //  myTrans.Commit();

                            resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };
                            emailHelper.SendEmailWithTemplatedAddUser(LoginModel.first_name, Convert.ToInt32(user_id), "Glamly");
                            resp.ResponseMessage = "User has been register successfully";
                            resp.ResponseCode = Response.Codes.OK;
                            return resp;
                        }
                        else
                        {
                            wp_users obj = new wp_users();
                            obj.user_email = LoginModel.user_email;
                            obj.user_login = LoginModel.user_login;
                            obj.user_nicename = LoginModel.user_nicename;
                            obj.user_pass = userPassword;
                            obj.user_registered = DateTime.Now;
                            obj.user_status = 1;
                            obj.user_url = "";
                            obj.user_activation_key = userSalt;
                            obj.display_name = "";

                            int user_id = _userService.Saveuserdata(obj);
                            LoginModel.ID = Convert.ToInt32(user_id);
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = LoginModel.user_type == "customer" ? "customer_logindata" : "pro_logindata";
                            obj1.meta_value = serialize.Serialize(userJsonObject);
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            // myTrans.Commit();
                            // resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };
                            emailHelper.SendEmailWithTemplatedAddUser(LoginModel.first_name,Convert.ToInt32(user_id), "Glamly");
                            resp.ResponseMessage = "User has been register successfully";
                            resp.ResponseCode = Response.Codes.OK;
                            return resp;

                        }
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    return resp;
                }

            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "register");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        #endregion

        #region "Services"

        /// <summary>
        /// Get Service list 
        /// </summary>
        /// <returns></returns>
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("serviceslist"), HttpGet]
        public ResponseExtended<List<wp_glamly_services>> GetServicesList()
        {
            var resp = new ResponseExtended<List<wp_glamly_services>>();
            var service = new List<wp_glamly_services>();
            try
            {
                service = _userService.GetServiceList();

                if (service != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = service;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("services"), HttpGet]
        public ResponseExtended<List<UserService>> GetServices()
        {

            var resp = new ResponseExtended<List<UserService>>();
            List<wp_glamly_services> servicesList = new List<wp_glamly_services>();
            List<wp_glamly_servicestypes> serviceTypeList = new List<wp_glamly_servicestypes>();
            List<UserService> serviceModelList = new List<UserService>();
            try
            {
                servicesList = _userService.GetServices();
                if (servicesList != null)
                {
                    foreach (var item in servicesList)
                    {
                        UserService serviceModel = new UserService();
                        serviceTypeList = _userService.GetTypesByServiceId(item.id);
                        List<Dictionary<string, string>> typeList = new List<Dictionary<string, string>>();
                        foreach (var type in serviceTypeList)
                        {
                            Dictionary<string, string> typeObject = new Dictionary<string, string>();
                            typeObject.Add("id", Convert.ToString(type.id));
                            typeObject.Add("typeName", type.typename);
                            typeObject.Add("price", Convert.ToString(type.price));
                            typeList.Add(typeObject);
                        }
                        serviceModel.id = item.id;
                        serviceModel.servicename = item.servicename;
                        serviceModel.status = item.status;
                        serviceModel.service_image = item.service_image;
                        serviceModel.service_type = typeList;
                        serviceModelList.Add(serviceModel);
                    }
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceModelList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get service by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("services/{id}"), HttpGet]
        public ResponseExtended<wp_glamly_services> GetServiceById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_services>();
            var service = new wp_glamly_services();
            try
            {
                service = _userService.GetServicesById(id);

                if (service != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = service;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        /// <summary>
        /// Get servives by type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("services/{id}/types"), HttpGet]
        public ResponseExtended<wp_glamly_servicestypes> GetServiceTypeById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_servicestypes>();
            var serviceType = new wp_glamly_servicestypes();
            try
            {
                serviceType = _userService.GetServiceTypeById(id);
                if (serviceType != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceType;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get types by service id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("types/{id}/services"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicestypes>> GetTypesByServiceId(int id)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicestypes>>();
            var serviceType = new List<wp_glamly_servicestypes>();
            try
            {
                serviceType = _userService.GetTypesByServiceId(id);
                if (serviceType != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = serviceType;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        #endregion

        #region "Bookings"
        /// <summary>
        /// Get all bookings of users
        /// </summary>
        /// <returns></returns>
        [CacheOutput(ClientTimeSpan = 10, ServerTimeSpan = 10, ExcludeQueryStringFromCacheKey = true)]
        [Route("bookings"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetBookings()
        {

            //Unable to cast object of type 'System.String' to type 'System.Collections.IList
            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                servicesbookingsList = _userService.GetBookings();
                if (servicesbookingsList != null)
                {
                    foreach (var item in servicesbookingsList)
                    {
                        string servicelist = string.Empty;
                        IEnumerable servicecollection = (IList)serialize.Deserialize(item.servicewithtypes);
                        item.servicewithtypes = Convert.ToString(servicecollection);
                    }
                    //foreach (var item in servicesbookingsList)
                    //{
                    //    string servicelist = string.Empty;                        
                    //    IEnumerable servicecollection = (IList)serialize.Deserialize(item.service);
                    //    foreach (var item1 in servicecollection)
                    //    {
                    //        servicelist += Convert.ToString(_userService.GetServicesById(Convert.ToInt32(item1)).servicename) + ",";
                    //    }

                    //    IEnumerable typecollection = (IList)serialize.Deserialize(item.type);
                    //    string servicetype = string.Empty;
                    //    foreach (var item2 in typecollection)
                    //    {
                    //        servicetype += Convert.ToString(_userService.GetServiceTypeById(Convert.ToInt32(item2)).typename) + ",";

                    //    }
                    //    item.service = servicelist.TrimEnd(',') as string;
                    //    item.type = servicetype.TrimEnd(',') as string;

                    //}
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Save the bookings 
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("addbookings"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SaveBookings(Booking bookings)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;

            try
            {
                if (bookings != null)
                {
                    jsonservices = serialize.Serialize(bookings.service);
                    jsontypes = serialize.Serialize(bookings.type);
                    jsonservicelist = JsonConvert.SerializeObject(bookings.servicewithtypes);
                    wp_glamly_servicesbookings booking = new wp_glamly_servicesbookings();
                    booking.address = !string.IsNullOrEmpty(bookings.address) ? bookings.address : string.Empty;
                    booking.altdatetime = !string.IsNullOrEmpty(bookings.altdatetime) ? bookings.altdatetime : string.Empty;
                    booking.service = !string.IsNullOrEmpty(jsonservices) ? jsonservices : string.Empty;
                    booking.type = !string.IsNullOrEmpty(jsontypes) ? jsontypes : string.Empty;
                    booking.billingaddress = !string.IsNullOrEmpty(bookings.billingaddress) ? bookings.billingaddress : string.Empty;
                    booking.bookingid = Helper.RandomString(9);
                    booking.servicewithtypes = !string.IsNullOrEmpty(serialize.Serialize(jsonservicelist)) ? serialize.Serialize(jsonservicelist) : string.Empty;
                    booking.city = !string.IsNullOrEmpty(bookings.city) ? bookings.city : string.Empty;
                    booking.datetime = !string.IsNullOrEmpty(bookings.datetime) ? bookings.datetime : string.Empty;
                    booking.email = !string.IsNullOrEmpty(bookings.email) ? bookings.email : string.Empty;
                    booking.firstname = !string.IsNullOrEmpty(bookings.firstname) ? bookings.firstname : string.Empty;
                    booking.surname = !string.IsNullOrEmpty(bookings.surname) ? bookings.surname : string.Empty;
                    booking.isedit = !string.IsNullOrEmpty(bookings.isedit) ? bookings.isedit : "false";
                    booking.zipcode = !string.IsNullOrEmpty(bookings.zipcode) ? bookings.zipcode : string.Empty;
                    booking.phone = !string.IsNullOrEmpty(bookings.phone) ? bookings.phone : string.Empty;
                    booking.newsletter = !string.IsNullOrEmpty(bookings.newsletter) ? bookings.newsletter : string.Empty;
                    booking.message = !string.IsNullOrEmpty(bookings.message) ? bookings.message : string.Empty;
                    booking.status = !string.IsNullOrEmpty(bookings.status) ? bookings.status : string.Empty;
                    booking.personal = !string.IsNullOrEmpty(bookings.personal) ? bookings.personal : string.Empty;
                    booking.isdeleted = !string.IsNullOrEmpty(bookings.isdeleted) ? bookings.isdeleted : "false";
                    booking.comments = !string.IsNullOrEmpty(bookings.comments) ? bookings.comments : string.Empty;
                    booking.workflowstatus = (int)BookingStatus.Draft;
                    booking.stylistid = 0;
                    booking.userid = bookings.userid > 0 ? bookings.userid : 0;
                    string bookingid = _userService.savebookingdata(booking);
                    wp_glamly_payment payment = new wp_glamly_payment();
                    payment.acquirer = !string.IsNullOrEmpty(bookings.payment.acquirer) ? bookings.payment.acquirer : string.Empty;
                    payment.amount = !string.IsNullOrEmpty(bookings.payment.amount) ? bookings.payment.amount : string.Empty;
                    payment.approvalcode = !string.IsNullOrEmpty(bookings.payment.approvalcode) ? bookings.payment.approvalcode : string.Empty;
                    payment.bookingid = !string.IsNullOrEmpty(bookingid) ? bookingid : string.Empty;
                    payment.calcfee = !string.IsNullOrEmpty(bookings.payment.calcfee) ? bookings.payment.calcfee : string.Empty;
                    payment.cardexpdate = !string.IsNullOrEmpty(bookings.payment.cardexpdate) ? bookings.payment.cardexpdate : string.Empty;
                    payment.cardnomask = !string.IsNullOrEmpty(bookings.payment.cardnomask) ? bookings.payment.cardnomask : string.Empty;
                    payment.cardprefix = !string.IsNullOrEmpty(bookings.payment.cardprefix) ? bookings.payment.cardprefix : string.Empty;
                    payment.cardtype = !string.IsNullOrEmpty(bookings.payment.cardtype) ? bookings.payment.cardtype : string.Empty;
                    payment.currency = !string.IsNullOrEmpty(bookings.payment.currency) ? bookings.payment.currency : string.Empty;
                    payment.dibsInternalIdentifier = !string.IsNullOrEmpty(bookings.payment.dibsInternalIdentifier) ? bookings.payment.dibsInternalIdentifier : string.Empty;
                    payment.fee = Convert.ToString(bookings.payment.fee);
                    payment.fullreply = !string.IsNullOrEmpty(bookings.payment.fullreply) ? bookings.payment.fullreply : string.Empty;
                    payment.lang = !string.IsNullOrEmpty(bookings.payment.lang) ? bookings.payment.lang : string.Empty;
                    payment.merchant = !string.IsNullOrEmpty(bookings.payment.merchant) ? bookings.payment.merchant : string.Empty;
                    payment.merchantid = Convert.ToString(bookings.payment.merchantid);
                    payment.method = !string.IsNullOrEmpty(bookings.payment.method) ? bookings.payment.method : string.Empty;
                    payment.mobilelib = !string.IsNullOrEmpty(bookings.payment.mobilelib) ? bookings.payment.mobilelib : string.Empty;
                    payment.orderid = !string.IsNullOrEmpty(bookings.payment.orderid) ? bookings.payment.orderid : string.Empty;
                    payment.paytype = !string.IsNullOrEmpty(bookings.payment.paytype) ? bookings.payment.paytype : string.Empty;
                    payment.platform = !string.IsNullOrEmpty(bookings.payment.platform) ? bookings.payment.platform : string.Empty;
                    payment.status = !string.IsNullOrEmpty(bookings.payment.status) ? bookings.payment.status : string.Empty;
                    payment.test = !string.IsNullOrEmpty(bookings.payment.test) ? bookings.payment.test : string.Empty;
                    payment.textreply = !string.IsNullOrEmpty(bookings.payment.textreply) ? bookings.payment.textreply : string.Empty;
                    payment.theme = !string.IsNullOrEmpty(bookings.payment.theme) ? bookings.payment.theme : string.Empty;
                    payment.timeout = !string.IsNullOrEmpty(bookings.payment.timeout) ? bookings.payment.timeout : string.Empty;
                    payment.transact = !string.IsNullOrEmpty(bookings.payment.transact) ? bookings.payment.transact : string.Empty;
                    payment.userid = bookings.userid > 0 ? bookings.userid : 0;
                    payment.version = !string.IsNullOrEmpty(bookings.payment.version) ? bookings.payment.version : string.Empty;
                    payment.isdeleted = !string.IsNullOrEmpty(bookings.isdeleted) ? bookings.isdeleted : "false";
                    payment.servicewithtypes = !string.IsNullOrEmpty(serialize.Serialize(jsonservicelist)) ? serialize.Serialize(jsonservicelist) : string.Empty;
                    payment.paymentdate = !string.IsNullOrEmpty(bookings.datetime) ? bookings.datetime : string.Empty;
                    var userid = _userService.savepaymentdata(payment);

                    if (userid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been successfully saved";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Update the booking by bookingid
        /// </summary>
        /// <param name="bookings"></param>
        /// <returns></returns>
        [Route("updatebookings"), HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateBookings(Booking bookings)
        {
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;
            jsonservices = serialize.Serialize(bookings.service);
            jsontypes = serialize.Serialize(bookings.type);
            jsonservicelist = JsonConvert.SerializeObject(bookings.servicewithtypes);
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            wp_glamly_servicesbookings bookingdetail = _userService.GetBookingById(Convert.ToInt32(bookings.bookingid));

            try
            {

                if (bookingdetail != null)
                {
                    bookingdetail.address = bookings.address;
                    bookingdetail.altdatetime = bookings.altdatetime;
                    bookingdetail.service = jsonservices;
                    bookingdetail.type = jsontypes;
                    bookingdetail.billingaddress = bookings.billingaddress;
                    bookingdetail.bookingid = Helper.RandomString(9); ;
                    bookingdetail.servicewithtypes = serialize.Serialize(jsonservicelist);
                    bookingdetail.city = bookings.city;
                    bookingdetail.datetime = bookings.datetime;
                    bookingdetail.email = bookings.email;
                    bookingdetail.firstname = bookings.firstname;
                    bookingdetail.surname = bookings.surname;
                    bookingdetail.isedit = "true";
                    bookingdetail.zipcode = bookings.zipcode;
                    bookingdetail.phone = bookings.phone;
                    bookingdetail.newsletter = bookings.newsletter;
                    bookingdetail.message = bookings.message;
                    bookingdetail.message = bookings.status;
                    bookingdetail.personal = bookings.personal;
                    bookingdetail.billingaddress = bookings.billingaddress;
                    bookingdetail.status = bookings.status;
                    bookingdetail.otherservices = bookings.otherservices;
                    var bookingid = _userService.updatebookingdata(bookingdetail);
                    if (bookingid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get all approved bookings of users
        /// </summary>
        /// <returns></returns>
       // [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("getapprovedbookings/{userid}"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetApprovedBookings(int userid)
        {

            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                servicesbookingsList = _userService.GetBookingByStatus(userid, "approved");

                if (servicesbookingsList != null)
                {

                    foreach (var item in servicesbookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = serialize.Deserialize(item.servicewithtypes);
                            item.servicewithtypes = (string)desearlize;
                        }
                    }

                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Get the cancel booking of users
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("getcancelbookings/{userid}"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetCancelBookings(int userid)
        {

            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var servicesbookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                servicesbookingsList = _userService.GetBookingByStatus(userid, "cancel");

                if (servicesbookingsList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = servicesbookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("bookings/{id}"), HttpGet]
        public ResponseExtended<wp_glamly_servicesbookings> GetBookingById(int id)
        {
            var resp = new ResponseExtended<wp_glamly_servicesbookings>();
            var bookingsList = new wp_glamly_servicesbookings();
            try
            {
                bookingsList = _userService.GetBookingById(id);
                if (bookingsList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = bookingsList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("usersbookings/{id}"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetBookingByUserId(int id)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var bookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                bookingsList = _userService.GetBookingByUserId(id);
                if (bookingsList != null)
                {

                    foreach (var item in bookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = serialize.Deserialize(item.servicewithtypes);
                            item.servicewithtypes = (string)desearlize;
                        }
                        if (!string.IsNullOrEmpty(item.otherservices))
                        {
                            var desearlizeotherservice = serialize.Deserialize(item.otherservices);
                            item.otherservices = (string)desearlizeotherservice;
                        }


                    }

                    if (bookingsList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = bookingsList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// delete booking by Id
        /// </summary>
        /// <param name="bookingid"></param>
        /// <returns></returns>
        /// 
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, ExcludeQueryStringFromCacheKey = true)]
        [Route("deletebooking/{bookingid}"), HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeleteBooking(string bookingid)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeleteBooking(bookingid);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "deletebooking");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        #endregion

        #region "Payment"
        /// <summary>
        /// Get payment recipt by userid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Route("payment/{id}"), HttpGet]
        public ResponseExtended<List<wp_glamly_payment>> GetPaymentById(int id)
        {

            var resp = new ResponseExtended<List<wp_glamly_payment>>();
            var paymentList = new List<wp_glamly_payment>();
            try
            {
                paymentList = _userService.GetPaymentById(id);
                if (paymentList != null)
                {

                    foreach (var item in paymentList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = serialize.Deserialize(item.servicewithtypes);
                            item.servicewithtypes = (string)desearlize;
                        }
                        if (!string.IsNullOrEmpty(item.otherservices))
                        {
                            var desearlizeotherservice = serialize.Deserialize(item.otherservices);
                            item.otherservices = (string)desearlizeotherservice;
                        }

                    }

                    if (paymentList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = paymentList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        [Route("payment"), HttpGet]
        public ResponseExtended<List<wp_glamly_payment>> GetPaymentList()
        {
            var resp = new ResponseExtended<List<wp_glamly_payment>>();
            var paymentList = new List<wp_glamly_payment>();
            try
            {
                paymentList = _userService.GetPaymentList();
                if (paymentList != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = paymentList;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }

        /// <summary>
        /// Delete the payment recipt.
        /// </summary>
        /// <param name="bookingid"></param>
        /// <returns></returns>
        [Route("deleterecipt/{bookingid}"), HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeletePaymentRecipt(string bookingid)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeletePaymentRecipt(bookingid);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "deletebooking");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        /// <summary>
        /// Update the service.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [Route("updateservices"), HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateService(Services service)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var servicename = _userService.GetServiceTypes().Where(s => s.id == service.Id);
                bool checkservicetype = _userService.GetServiceTypes().Exists(x => x.typename == service.typeName && x.id == service.Id);
                int result = 0;
                if (checkservicetype == false)
                {
                    wp_glamly_servicestypes obj = new wp_glamly_servicestypes();
                    obj.typename = service.typeName;
                    obj.price = service.Price;
                    obj.id = service.Id;
                    obj.serviceid = servicename.FirstOrDefault().serviceid;
                    obj.status = servicename.FirstOrDefault().status;
                    result = _userService.updateservicetypes(obj);
                    if (result > 0)
                        resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "UpdateService");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Add the service type.
        /// </summary>
        /// <param name="ServiceType"></param>
        /// <param name="Price"></param>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        [Route("addservicestypes"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddServiceType(string ServiceType, int Price, string ServiceName)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                bool checkservicetype = _userService.GetServiceTypes().Exists(x => x.typename == ServiceName);
                int result = 0;
                if (checkservicetype == false)
                {
                    wp_glamly_servicestypes obj = new wp_glamly_servicestypes();
                    obj.typename = ServiceType;
                    obj.price = Price;
                    obj.status = 1;
                    obj.serviceid = _userService.GetIdByName(ServiceName);
                    result = _userService.SaveServicetype(obj);
                    if (result > 0)
                        resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.Exists;
                }
                return resp;
            }
            catch (Exception ex)
            {

                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "AddServiceType");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Delete the service by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteservices"), HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeleteService(int id)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeleteServiceType(id);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "DeleteServiceType");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Get all assigned booking by admin
        /// </summary>
        /// <param name="stylistid"></param>
        /// <returns></returns>
        [Route("assignedbookingbyadmin/{stylistid}"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> AssignedBookingByAdmin(int stylistid)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var bookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                bookingsList = _userService.GetApprovedBookingByAdmin(stylistid);

                if (bookingsList != null)
                {

                    foreach (var item in bookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlizeservicetype = serialize.Deserialize(item.servicewithtypes);
                            item.servicewithtypes = (string)desearlizeservicetype;
                        }
                        if (!string.IsNullOrEmpty(item.otherservices))
                        {
                            var desearlizeotherservice = serialize.Deserialize(item.otherservices);
                            item.otherservices = (string)desearlizeotherservice;
                        }

                    }

                    if (bookingsList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = bookingsList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByAdmin");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        /// <summary>
        /// Accepted booking by pro user 
        /// </summary>
        /// <param name="bookingid"></param>
        /// <returns></returns>
        [Route("acceptedbookingbypro/{bookingid}"), HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> AcceptedBookingByPro(string bookingid)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();

            try
            {
                var booking = _userService.AcceptBookingByPro(bookingid);
                if (!string.IsNullOrEmpty(booking))
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByPro");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        /// <summary>
        /// Get approved booking by Pro"
        /// </summary>   
        /// <returns></returns>
        [Route("getapprovedbookingbypro/{stylistid}"), HttpGet]
        public ResponseExtended<List<wp_glamly_servicesbookings>> GetApprovedBookingByPro(int stylistid)
        {
            var resp = new ResponseExtended<List<wp_glamly_servicesbookings>>();
            var bookingsList = new List<wp_glamly_servicesbookings>();
            try
            {
                bookingsList = _userService.GetApprovedBookingByPro(stylistid);
                if (bookingsList != null)
                {

                    foreach (var item in bookingsList)
                    {
                        if (!string.IsNullOrEmpty(item.servicewithtypes))
                        {
                            var desearlize = serialize.Deserialize(item.servicewithtypes);
                            item.servicewithtypes = (string)desearlize;
                        }
                        if (!string.IsNullOrEmpty(item.otherservices))
                        {
                            var desearlizeotherservice = serialize.Deserialize(item.otherservices);
                            item.otherservices = (string)desearlizeotherservice;
                        }

                    }

                    if (bookingsList.Count > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = bookingsList;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByPro");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Accepted booking by user and update payment recipt
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [Route("acceptbookingbyuser"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AcceptedBookingByUser(PaymentReceipt payment)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;
            try
            {
                if (payment != null)
                {
                    jsonservicelist = JsonConvert.SerializeObject(payment.otherservices);
                    wp_glamly_payment wp_payment = new wp_glamly_payment();
                    wp_payment.acquirer = !string.IsNullOrEmpty(payment.acquirer) ? payment.acquirer : string.Empty;
                    wp_payment.amount = !string.IsNullOrEmpty(payment.amount) ? payment.amount : string.Empty;
                    wp_payment.approvalcode = !string.IsNullOrEmpty(payment.approvalcode) ? payment.approvalcode : string.Empty;
                    wp_payment.bookingid = !string.IsNullOrEmpty(payment.bookingid) ? payment.bookingid : string.Empty;
                    wp_payment.calcfee = !string.IsNullOrEmpty(payment.calcfee) ? payment.calcfee : string.Empty;
                    wp_payment.cardexpdate = !string.IsNullOrEmpty(payment.cardexpdate) ? payment.cardexpdate : string.Empty;
                    wp_payment.cardnomask = !string.IsNullOrEmpty(payment.cardnomask) ? payment.cardnomask : string.Empty;
                    wp_payment.cardprefix = !string.IsNullOrEmpty(payment.cardprefix) ? payment.cardprefix : string.Empty;
                    wp_payment.cardtype = !string.IsNullOrEmpty(payment.cardtype) ? payment.cardtype : string.Empty;
                    wp_payment.currency = !string.IsNullOrEmpty(payment.currency) ? payment.currency : string.Empty;
                    wp_payment.dibsInternalIdentifier = !string.IsNullOrEmpty(payment.dibsInternalIdentifier) ? payment.dibsInternalIdentifier : string.Empty;
                    wp_payment.fee = Convert.ToString(payment.fee);
                    wp_payment.fullreply = !string.IsNullOrEmpty(payment.fullreply) ? payment.fullreply : string.Empty;
                    wp_payment.lang = !string.IsNullOrEmpty(payment.lang) ? payment.lang : string.Empty;
                    wp_payment.merchant = !string.IsNullOrEmpty(payment.merchant) ? payment.merchant : string.Empty;
                    wp_payment.merchantid = Convert.ToString(payment.merchantid);
                    wp_payment.method = !string.IsNullOrEmpty(payment.method) ? payment.method : string.Empty;
                    wp_payment.mobilelib = !string.IsNullOrEmpty(payment.mobilelib) ? payment.mobilelib : string.Empty;
                    wp_payment.orderid = !string.IsNullOrEmpty(payment.orderid) ? payment.orderid : string.Empty;
                    wp_payment.paytype = !string.IsNullOrEmpty(payment.paytype) ? payment.paytype : string.Empty;
                    wp_payment.platform = !string.IsNullOrEmpty(payment.platform) ? payment.platform : string.Empty;
                    wp_payment.status = !string.IsNullOrEmpty(payment.status) ? payment.status : string.Empty;
                    wp_payment.test = !string.IsNullOrEmpty(payment.test) ? payment.test : string.Empty;
                    wp_payment.textreply = !string.IsNullOrEmpty(payment.textreply) ? payment.textreply : string.Empty;
                    wp_payment.theme = !string.IsNullOrEmpty(payment.theme) ? payment.theme : string.Empty;
                    wp_payment.timeout = !string.IsNullOrEmpty(payment.timeout) ? payment.timeout : string.Empty;
                    wp_payment.transact = !string.IsNullOrEmpty(payment.transact) ? payment.transact : string.Empty;
                    wp_payment.userid = payment.userid > 0 ? payment.userid : 0;
                    wp_payment.version = !string.IsNullOrEmpty(payment.version) ? payment.version : string.Empty;
                    wp_payment.servicewithtypes = null;
                    wp_payment.isdeleted = !string.IsNullOrEmpty(payment.isdeleted) ? payment.isdeleted : "false";
                    wp_payment.otherservices = !string.IsNullOrEmpty(serialize.Serialize(jsonservicelist)) ? serialize.Serialize(jsonservicelist) : string.Empty;
                    wp_payment.paymentdate = !string.IsNullOrEmpty(payment.paymentdate) ? payment.paymentdate : string.Empty;
                    var userid = _userService.savepaymentdata(wp_payment);
                    _userService.AcceptedBookingByUser(payment.bookingid);

                    if (userid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                    }

                    else
                    {
                        resp.ResponseCode = Response.Codes.NotFound;
                    }

                }
                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByPro");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }
        /// <summary>
        /// Get booking count by user
        /// </summary>
        /// <returns></returns>
        [Route("getcountofuserbooking/{userid}"), HttpGet]
        public int GetBookingCountByUser(int userid)
        {
            int bookingcount = 0;
            try
            {
                bookingcount = _userService.GetBookingCountByUserId(userid);
                return bookingcount;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByPro");
                return 0;
            }

        }

        /// <summary>
        /// update the otherservice of booking
        /// </summary>
        /// <param name="updateotherservice"></param>
        /// <returns></returns>
        [Route("updateotherservices"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> Updateotherservices(updatebooking updateotherservice)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            // wp_glamly_servicesbookings bookingdetail = _userService.GetBookingById(Convert.ToInt32(bookingid));
            string jsonservicelist = string.Empty;
            string otherservice = string.Empty;
            jsonservicelist = JsonConvert.SerializeObject(updateotherservice.otherservices);
            otherservice = serialize.Serialize(jsonservicelist);

            try
            {
                if (updateotherservice.bookingid != null)
                {
                    var id = _userService.Updateotherservices(updateotherservice.bookingid, otherservice);
                    if (id > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updatebookings");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }
        }


        /// <summary>
        /// Get the underlagdata by pro 
        /// </summary>
        /// <param name="stylistid"></param>
        /// <returns></returns>
        [Route("getUnderlagdataByPro/{stylistid}"), HttpGet]
        public ResponseExtended<UnderLag> GetUnderlagdataByPro(int stylistid)
        {
            int totalbooking = 0, totalincome = 0, totalassign = 0;
            var resp = new ResponseExtended<UnderLag>();
            UnderLag obj = new UnderLag();
            wp_glamly_payment objpay = new wp_glamly_payment();
            // var servicesbookingsList = new List<wp_glamly_servicesbookings>();

            try
            {
                totalbooking = _userService.GetBookingCountByStylistId(stylistid);

                List<wp_glamly_servicesbookings> bookingids = _userService.GetBookingIdByPro(stylistid);

                if (bookingids != null)
                {
                    foreach (var item in bookingids)
                    {

                        totalincome += Convert.ToInt32(_userService.GetUnderlagdata(item.bookingid));

                        objpay = _userService.GetUnderlagPaymentdata(item.bookingid);
                        if (objpay != null && objpay.otherservices != null)
                        {
                            totalassign += Convert.ToInt32(objpay.amount);
                        }

                    }
                }

                obj.BookingsCount = totalbooking;
                obj.TotalIncome = totalincome;
                obj.StatisticsCount = totalassign;

                if (obj != null)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    resp.ResponseData = obj;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "GetApprovedBookingByPro");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Delete single payment receipt by id
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [Route("deletePaymentReciptById/{paymentId}"), HttpDelete]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeletePaymentReciptById(int paymentId)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeletePaymentReceiptById(paymentId);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "deletePaymentReciptById");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }

        /// <summary>
        /// Decline the booking by pro user.
        /// </summary>
        /// <param name="BookingId"></param>
        /// <returns></returns>
        [Route("declineBookingByProUser/{BookingId}"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> DeclineBookingByProUser(string BookingId)
        {
            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var isdelete = _userService.DeclineBookingByProUser(BookingId);
                if (isdelete)
                {
                    resp.ResponseCode = Response.Codes.OK;
                    emailHelper.SendEmailToAdminCancelBookingByPro(BookingId,"Booking is cancelled");
                }

                else
                {
                    resp.ResponseCode = Response.Codes.NotFound;
                }
                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "declineBookingByProUser");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }



        #region "Puch Notification"
        /// <summary>
        /// Update device token of user.
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="devicetoken"></param>
        /// <returns></returns>
        [Route("updateDeviceTokenByUserId"), HttpPut]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateDeviceTokenByUserId(int userid, string devicetoken)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(userid);
                if (usermetadata != null)
                {
                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                    if (usercollection != null)
                    {
                        usercollection.devicetoken = !string.IsNullOrEmpty(devicetoken) ? devicetoken : string.Empty;
                        var jsonupdate = JsonConvert.SerializeObject(usercollection);
                        usermetadata.meta_value = serialize.Serialize(jsonupdate);
                        int user_id = _userService.updateuserdata(usermetadata);

                        if (user_id > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "User has been updated successfully";
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                        }

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "updateUser");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }
        #endregion


        #region "Forget Password"     
        /// <summary>
        /// Send forget password email.
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>     
        [Route("SendForgetPasswordEmail/{emailId}"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> SendForgetPasswordEmail(string emailId)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            if (emailId == null)
            {
                resp.ResponseMessage = "provided parameters are incorrect";
                resp.ResponseCode = Response.Codes.InvalidRequest;
            }
            else
            {
                var userdetail = _userService.GetUserByEmailId(emailId);

                if (userdetail != null)
                {
                    emailHelper.SendEmailWithTemplateResetPasswordUser(Convert.ToInt32(userdetail.ID), "Glamly");
                    resp.ResponseCode = Response.Codes.OK;
                }
                else
                {
                    resp.ResponseCode = Response.Codes.InvalidUser;
                    resp.ResponseMessage = "Invalid email id";
                }
            }
            return resp;
        }
        #endregion


        #region "Stylist  Schedule"
        /// <summary>
        /// Add update avaiable stylist dates.
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        /// 
        [Route("addUpdateAvailsStylistDates"), HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> AddUpdateAvailsStylistDates(AvailsDates dates)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            List<StylistDates> addlist = new List<StylistDates>();
            List<StylistDates> deletelist = new List<StylistDates>();
            try
            {
                if (dates != null)
                {

                    foreach (var item in dates.dates)
                    {
                        StylistDates objadd = new StylistDates();
                        if (item.status == "Add")
                        {
                            objadd.stylistid = dates.stylistid;
                            objadd.date = item.date;
                            objadd.status = item.status;
                            objadd.isdeleted = "false";
                            addlist.Add(objadd);
                        }
                    }
                    if (addlist != null)
                    {
                        _userService.SaveStylistAvailsDates(addlist);
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }

                   
                    foreach (var item in dates.dates)
                    {
                        StylistDates objdelete = new StylistDates();
                        if (item.status == "Delete")
                        {
                            objdelete.id = item.id;
                            objdelete.stylistid = item.stylistid;
                            objdelete.date = item.date;
                            objdelete.status = item.status;
                            objdelete.isdeleted = "true";
                            deletelist.Add(objdelete);
                        }

                    }
                    if (deletelist != null)
                    {
                        _userService.DeleteStylistAvailsDates(deletelist);
                    }
                    else
                    {
                        resp.ResponseMessage = "Provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }
                else
                {
                    resp.ResponseMessage = "Provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }

                return resp;
            }
            catch (Exception ex)
            {
                Logs.MailError(ex, "Glamly API Ver 1.0");
                Logs.Add(string.Format("Exception-{0}::Error={1}", ex.TargetSite.Name, ex.ToString()), "services/{id}/types");
                resp.ResponseCode = Response.Codes.InternalServerError;
                return resp;
            }

        }


        #endregion


        #endregion
    }
}
