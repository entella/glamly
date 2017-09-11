using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GlamlyWebAPI.Models;
using System.Security.Cryptography;
using System.Net.Http.Formatting;
using GlamlyWebAPI.Library;
using GlamlyServices.Services;
using GlamlyData;
using NLog;
using GlamlyData.Entities;
using System.Web.Script.Serialization;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

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
        Helper helpers = new Helper();
        ResponseExtended<string> resp = new ResponseExtended<string>();
        Serializer serialize = new Serializer();
        JavaScriptSerializer javaserializer = new JavaScriptSerializer();
        #endregion

        #region "User"

        /// <summary>
        /// Get the user list.
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("Allusers")]
        [HttpGet]
        public List<wp_users> GetUser()
        {
            try
            {
                _logger.Info("Get all users list");
                return _userService.GetUser();
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }

        }


        /// <summary>
        /// Get the user details by id.
        /// </summary>
        /// <returns></returns>
        /// 
       //[Authorize]
        [Route("userdetail")]
        [HttpGet]
        public ResponseExtended<System.Web.Mvc.JsonResult> GetUserdetails()
        {
            try
            {
                Serializer serialize = new Serializer();
                _logger.Info("Get users by id");
                var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
                var userdetail = _userService.GetUserByEmailId("krnrana@gmail.com");
                if (userdetail != null)
                {
                    List<wp_usermeta> usermeta = new List<wp_usermeta>();
                    List<UserData> userdatalist = new List<UserData>();
                    int userid = Convert.ToInt32(userdetail.ID);
                    usermeta = _userService.GetUserMetadatabyId(userid);

                    foreach (var item in usermeta)
                    {

                        UserData objuser = new UserData();
                        decimal metaid = item.umeta_id;
                        string metavalue = item.meta_key;
						
					

                        if (metavalue == "wp_capabilities")
                        {
                            objuser.user_type = item.meta_key == "wp_capabilities" ? Convert.ToString(serialize.Deserialize(item.meta_value)) : "";
                        }
                        else if (metavalue == "first_name")
                        {
                            objuser.first_name = item.meta_key == "first_name" ? item.meta_value : "";
                        }
                        else if (metavalue == "last_name")
                        {
                            objuser.last_name = item.meta_key == "last_name" ? item.meta_value : "";
                        }
                        else if (metavalue == "mobilenumber")
                        {
                            objuser.last_name = item.meta_key == "last_name" ? item.meta_value : "";
                        }
                        else if (metavalue == "offer")
                        {
                            objuser.offer = item.meta_key == "offer" ? Convert.ToBoolean(item.meta_value) : true;
                        }
                        else if (metavalue == "upcomingbookings")
                        {
                            objuser.upcomingbookings = item.meta_key == "upcomingbookings" ? Convert.ToBoolean(item.meta_value) : true;
                        }
                        else if (metavalue == "notificationall")
                        {
                            objuser.notificationall = item.meta_key == "notificationall" ? Convert.ToBoolean(item.meta_value) : true;
                        }
                        else
                        {
                            resp.ResponseCode = Response.Codes.InvalidRequest;
                            resp.ResponseMessage = "User is not authenticated by API.";
                        }
                        //switch (metavalue)
                        //{
                        //    case "wp_capabilities":
                        //        objuser.user_type = item.meta_key == "wp_capabilities" ? Convert.ToString(serialize.Deserialize(item.meta_value)) : "";
                        //        break;
                        //    case "first_name":
                        //        objuser.first_name = item.meta_key == "first_name" ? item.meta_value : "";
                        //        break;
                        //    case "last_name":
                        //        objuser.last_name = item.meta_key == "last_name" ? item.meta_value : "";
                        //        break;
                        //    case "mobilenumber":
                        //        objuser.mobile = item.meta_key == "mobilenumber" ? item.meta_value : "";
                        //        break;
                        //    case "offer":
                        //        objuser.offer = item.meta_key == "offer" ? Convert.ToBoolean(item.meta_value) : true;
                        //        break;
                        //    case "upcomingbookings":
                        //        objuser.upcomingbookings = item.meta_key == "upcomingbookings" ? Convert.ToBoolean(item.meta_value) : true;
                        //        break;
                        //    case "notificationall":
                        //        objuser.notificationall = item.meta_key == "notificationall" ? Convert.ToBoolean(item.meta_value) : true;
                        //        break;
                        //    default:
                        //        Console.WriteLine("The input is not valid.");
                        //        break;
                        //}


                        //  UserData objuser = new UserData();
                        //objuser.user_email = userdetail.user_email;
                        //objuser.user_type = item.meta_key == "wp_capabilities" ? Convert.ToString(serialize.Deserialize(item.meta_value)) : "";
                        //objuser.first_name = item.meta_key == "first_name" ? item.meta_value : "";
                        //objuser.last_name = item.meta_key == "last_name" ? item.meta_value : "";
                        //objuser.mobile = item.meta_key == "mobilenumber" ? item.meta_value : "";
                        //objuser.offer = item.meta_key == "offer" ? Convert.ToBoolean(item.meta_value) : true;
                        //objuser.upcomingbookings = item.meta_key == "upcomingbookings" ? Convert.ToBoolean(item.meta_value) : true;
                        //objuser.notificationall = item.meta_key == "notificationall" ? Convert.ToBoolean(item.meta_value) : true;
                        userdatalist.Add(objuser);
                        new System.Web.Mvc.JsonResult { Data = new { userData = userdatalist } };
                    }
                }

                resp.ResponseCode = Response.Codes.OK;
                return resp;
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }

        }



        /// <summary>
        /// Get the stylist pro user list.
        /// </summary>
        /// <returns></returns>
        [Route("prousers")]
        [HttpGet]
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
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
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
        [Route("login")]
        [HttpPost]
        public ResponseExtended<string> UserLogin(UserData user)
        {
          //  string baseAddress = "http://192.168.1.120:8010";

            string baseAddress = "http://localhost:51458";
            Token token = new Token();
            //  var resp = new ResponseExtended<string>();
            // Logs.Add("Log - Method:UserLogin");
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.user_email))
                {
                    if (string.IsNullOrEmpty(user.user_email) || !Helper.IsEmail(user.user_email.Replace("'", "''")))
                    {
                        ResponseExtended<string> resp = new ResponseExtended<string>();
                        resp.ResponseCode = Response.Codes.InvalidEmailAddress;
                        resp.ResponseMessage = "Email is not valid";
                        return resp;
                    }
                }
                if (string.IsNullOrEmpty(user.user_pass))
                {
                    ResponseExtended<string> resp = new ResponseExtended<string>();
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    resp.ResponseMessage = "password cannot be empty";
                    return resp;
                }

                using (var client = new HttpClient())
                {
                    //var form1 = new Dictionary<string, string>
                    //   {
                    //       {"grant_type", "password"},
                    //       {"username", user.user_email},
                    //       {"password", user.user_pass},
                    //       {"usertype", user.user_type},
                    //       {"facebookid", user.user_facebookid},
                    //   };

                    var form = new Dictionary<string, string>
                       {
                           {"grant_type", "password"},
                           {"username", user.user_email},
                           {"password", user.user_pass},
                           {"usertype", user.user_type},
                       };
                    var tokenResponse = client.PostAsync(baseAddress + "/AuthenticationToken", new FormUrlEncodedContent(form)).Result;


                    token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                    if (string.IsNullOrEmpty(token.Error))
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Token has been created successfully.";
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.ApiUnauthorized;
                        resp.ResponseMessage = "User is not authenticated by API.";
                    }




                }
                resp.ResponseData = token.AccessToken;
                //resp.ResponseCode = Response.Codes.OK;
            }
            return resp;
        }

    
        /// <summary>
        /// Register the user 
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public ResponseExtended<System.Web.Mvc.JsonResult> RegisterUser(UserData LoginModel)
        {

            var resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            string userSalt = string.Empty;
            string userPassword = string.Empty;
            string hashedPassword = string.Empty;
            string name = string.Empty;
            string customernumber = string.Empty;

           
            userSalt = Helper.getPasswordSalt();

            string userJsonObject = JsonConvert.SerializeObject(LoginModel);


            if (!string.IsNullOrEmpty(LoginModel.user_pass))
            {
                userPassword = Hashing.MD5Hash(LoginModel.user_pass.Trim(), userSalt);
            }


            //MySqlConnection connectionString = new MySqlConnection("server=52.209.186.41;user id=usrglamly;password=G@Lm!Y;database=wp_glamly;");
            //connectionString.Open();
            //MySqlTransaction myTrans = connectionString.BeginTransaction();

            if (LoginModel != null)
            {
                try
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
                                    IsUserEmailExist = GetUser().Exists(users => users.user_email == LoginModel.user_email && usercollection.user_type == LoginModel.user_type);
                                }

                            }
                        }
                    }

                    if (IsFacebookIdExist || IsUserEmailExist)
                    {
                        resp.ResponseMessage = "User already exist.Please try another";
                        resp.ResponseCode = Response.Codes.AlreadyExist;
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
                            obj.user_registered = DateTime.Now;
                            obj.user_status = 1;
                            obj.user_url = "";
                            obj.user_activation_key = "";
                            obj.display_name = "";

                            int user_id = _userService.Saveuserdata(obj);
                            LoginModel.ID = Convert.ToInt32(user_id);
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = "loginfbdetail";
                            obj1.meta_value = serialize.Serialize(userJsonObject); //"a:1:{s:13:" + "\"" + LoginModel.user_type + "\"" + ";b:1;}";
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            //  myTrans.Commit();
                            resp.ResponseCode = Response.Codes.Success;
                            resp.ResponseMessage = "User has been register through facebook successfully";
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

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = "logindetail";
                            obj1.meta_value = serialize.Serialize(userJsonObject); //"a:1:{s:13:" + "\"" + LoginModel.user_type + "\"" + ";b:1;}";
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            // myTrans.Commit();
                            resp.ResponseCode = Response.Codes.Success;
                            resp.ResponseMessage = "User has been register successfully";
                            resp.ResponseCode = Response.Codes.OK;
                            return resp;

                        }
                    }
                }
                catch (Exception exception)
                {
                    // myTrans.Rollback();
                    resp.ResponseMessage = "Inputs parameter has null value";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                    _logger.Info(string.Format("exception {0}", exception.Message));
                    _logger.Info(string.Format("exception {0}", exception.Source));
                    return resp;
                }
            }
            else
            {
                resp.ResponseMessage = "Inputs parameter has null value";
                resp.ResponseCode = Response.Codes.InvalidRequest;
                return resp;
            }
        }

        #endregion


        #region "Services"
        /// <summary>
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        [Route("services")]
        [HttpGet]
        public List<UserService> GetServices()
        {
            List<wp_glamly_services> servicesList = new List<wp_glamly_services>();
            List<wp_glamly_servicestypes> serviceTypeList = new List<wp_glamly_servicestypes>();
            List<UserService> serviceModelList = new List<UserService>();
            try
            {
                servicesList = _userService.GetServices();
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
                        typeList.Add(typeObject);
                    }
                    serviceModel.id = item.id;
                    serviceModel.servicename = item.servicename;
                    serviceModel.status = item.status;
                    serviceModel.service_image = item.service_image;
                    serviceModel.service_type = typeList;
                    serviceModelList.Add(serviceModel);
                }
                return serviceModelList;
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }
        /// <summary>
        /// Get service by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Route("services/{id}")]
        [HttpGet]
        public wp_glamly_services GetServiceById(int id)
        {
            try
            {
                return _userService.GetServicesById(id);
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }


        /// <summary>
        /// Get servives by type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("services/{id}/types")]
        [HttpGet]
        public wp_glamly_servicestypes GetServiceTypeById(int id)
        {
            try
            {
                return _userService.GetServiceTypeById(id);
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }
        /// <summary>
        /// Get types by service id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("types/{id}/services")]
        [HttpGet]
        public List<wp_glamly_servicestypes> GetTypesByServiceId(int id)
        {
            try
            {
                return _userService.GetTypesByServiceId(id);
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }

        #endregion


        #region "Bookings"
        /// <summary>
        /// Get all bookings of users
        /// </summary>
        /// <returns></returns>
        [Route("bookings")]
        [HttpGet]
        public List<wp_glamly_servicesbookings> GetBookings()
        {

            //wp_glamly_services servicesListobj = new wp_glamly_services();
            //List<wp_glamly_servicestypes> serviceTypeList = new List<wp_glamly_servicestypes>();
            //wp_glamly_servicestypes serviceTypeListobj = new wp_glamly_servicestypes();
            //List<UserService> serviceModelList = new List<UserService>();           
            //List<string> serviceids = new List<string>();
            //List<object> servicenames = new List<object>();
            try
            {
                List<wp_glamly_servicesbookings> servicesbookingsList = new List<wp_glamly_servicesbookings>();
                //List<wp_glamly_services> servicesList = new List<wp_glamly_services>();
                servicesbookingsList = _userService.GetBookings();
                foreach (var item in servicesbookingsList)
                {
                    string servicelist = string.Empty;
                    wp_glamly_servicesbookings servicesbookingsListobj = new wp_glamly_servicesbookings();
                    IEnumerable servicecollection = (IList)serialize.Deserialize(item.service);
                    foreach (var item1 in servicecollection)
                    {                        
                       servicelist += Convert.ToString(_userService.GetServicesById(Convert.ToInt32(item1)).servicename) + ",";
                    }

                    IEnumerable typecollection = (IList)serialize.Deserialize(item.type);
                    string servicetype = string.Empty;
                    foreach (var item2 in typecollection)
                    {
                     //   serviceTypeListobj = _userService.GetServiceTypeById((Convert.ToInt32(item2)));
                        servicetype += Convert.ToString(_userService.GetServiceTypeById(Convert.ToInt32(item2)).typename) + ",";

                    }
                    item.service = servicelist.TrimEnd(',');
                    item.type = servicetype.TrimEnd(',');
                   
                }
                return servicesbookingsList;
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }

        /// <summary>
        /// Get all approved bookings of users
        /// </summary>
        /// <returns></returns>
        [Route("bookings/status/{status}")]
        [HttpGet]
        public List<wp_glamly_servicesbookings> GetApprovedBookings(string status)
        {
            try
            {
                return _userService.GetBookingByStatus(status);
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Route("bookings/{id}")]
        [HttpGet]
        public wp_glamly_servicesbookings GetBookingById(int id)
        {
            try
            {
                return _userService.GetBookingById(id);
            }
            catch (Exception exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                resp.ResponseMessage = "User is not authenticated by API.";
                _logger.Info(string.Format("exception {0}", exception.Message));
                _logger.Info(string.Format("exception {0}", exception.Source));
                return null;
            }
        }
        #endregion


    }
}
