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
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

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
        [Route("userdetail/{id}")]
        [HttpGet]
        public ResponseExtended<UserData> GetUserdetails(int id)
        {
            try
            {
                _logger.Info("Get users by id");
                var resp = new ResponseExtended<UserData>();

                var usermetadata = _userService.GetUserMetadatakeybyId(id);
                if (usermetadata != null)
                {
                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                    if (usercollection != null)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseData = usercollection;
                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                        resp.ResponseData = usercollection;
                    }
                }

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
        /// 
        /// </summary>
        /// <param name="userdata"></param>
        /// <returns></returns>
        [Route("updateUser")]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateUserDetails(UserData userdata)
        {
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            try
            {
                var usermetadata = _userService.GetUserMetadatakeybyId(userdata.ID);
                if (usermetadata != null)
                {
                    var desearlize = serialize.Deserialize(usermetadata.meta_value);
                    UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));

                    if (usercollection != null)
                    {
                        usercollection.first_name = userdata.first_name;
                        usercollection.last_name = userdata.last_name;
                        usercollection.mobile = userdata.mobile;
                        usercollection.user_email = userdata.user_email;
                        usercollection.offer = userdata.offer;
                        usercollection.upcomingbookings = userdata.upcomingbookings;
                        usercollection.notificationall = userdata.notificationall;

                        var jsonupdate = JsonConvert.SerializeObject(usercollection);

                        usermetadata.meta_value = serialize.Serialize(jsonupdate);
                        int userid = _userService.updateuserdata(usermetadata);


                        if (userid > 0)
                        {
                            resp.ResponseCode = Response.Codes.OK;
                            resp.ResponseMessage = "user has been updated successfully";
                        }
                        else
                        {
                            resp.ResponseMessage = "provided parameters are incorrect";
                            resp.ResponseCode = Response.Codes.InternalServerError;
                        }

                    }
                    else
                    {
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                    }
                }

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



            return resp;
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
        public ResponseExtended<Dictionary<string, string>> UserLogin(UserData user)
        {
            string baseAddress = "http://192.168.1.120:8010";
            ResponseExtended<Dictionary<string, string>> resp = new ResponseExtended<Dictionary<string, string>>();
              //string baseAddress = "http://localhost:51458";
            Token token = new Token();           
            try
            {
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.user_email))
                    {
                        if (string.IsNullOrEmpty(user.user_email) || !Helper.IsEmail(user.user_email.Replace("'", "''")))
                        {
                            //  ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
                            resp.ResponseCode = Response.Codes.InvalidEmailAddress;
                            resp.ResponseMessage = "Email is not valid";
                            return resp;
                        }
                    }
                    if (string.IsNullOrEmpty(user.user_pass) && string.IsNullOrEmpty(user.user_facebookid))
                    {
                        // ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
                        resp.ResponseCode = Response.Codes.InvalidRequest;
                        resp.ResponseMessage = "password cannot be empty";
                        return resp;
                    }

                    using (var client = new HttpClient())
                    {
                        var form = new Dictionary<string, string>
                       {
                           {"grant_type", "password"},
                           {"username", user.user_email},
                           {"password", user.user_pass},
                           {"usertype", user.user_type},
                           {"facebookid", user.user_facebookid},
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
                    Dictionary<string, string> typeObject = new Dictionary<string, string>();
                    typeObject.Add("Token", Convert.ToString(token.AccessToken));
                    typeObject.Add("Userid", Convert.ToString(token.Id));
                    typeObject.Add("FirstName", Convert.ToString(token.FirstName));
                    typeObject.Add("UserEmail", Convert.ToString(user.user_email));                    
                    typeObject.Add("Mobile", Convert.ToString(token.Mobile));
                    resp.ResponseData = typeObject;
                    resp.ResponseCode = Response.Codes.OK;
                }
                return resp;
            }
            catch (Exception ex)
            {
                resp.ResponseMessage = "User is not authenticated by API.";
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
                return resp;
            }
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
            string userJsonObject = string.Empty;

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

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = "loginfbdetail";
                            obj1.meta_value = serialize.Serialize(userJsonObject); //"a:1:{s:13:" + "\"" + LoginModel.user_type + "\"" + ";b:1;}";
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            //  myTrans.Commit();
                            resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };
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
                            LoginModel.offer = true;
                            LoginModel.upcomingbookings = true;
                            LoginModel.notificationall = true;

                            userJsonObject = JsonConvert.SerializeObject(LoginModel);

                            var obj1 = new wp_usermeta();
                            obj1.user_id = LoginModel.ID;
                            obj1.meta_key = "logindetail";
                            obj1.meta_value = serialize.Serialize(userJsonObject); //"a:1:{s:13:" + "\"" + LoginModel.user_type + "\"" + ";b:1;}";
                            int metauser_id = _userService.Saveusermedadata(obj1);

                            // myTrans.Commit();
                            resp.ResponseData = new System.Web.Mvc.JsonResult { Data = LoginModel.ID };

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
            try
            {
                List<wp_glamly_servicesbookings> servicesbookingsList = new List<wp_glamly_servicesbookings>();
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("addbookings")]
        [HttpPost]
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
                    jsonservicelist = JsonConvert.SerializeObject(bookings.servicesWithType);
                    wp_glamly_servicesbookings obj = new wp_glamly_servicesbookings();
                    obj.address = bookings.address;
                    obj.altdatetime = bookings.altdatetime;
                    obj.service = jsonservices;
                    obj.type = jsontypes;
                    obj.billingaddress = bookings.billingaddress;
                    obj.bookingid = Helper.RandomString(9); ;
                    obj.servicesWithType = serialize.Serialize(jsonservicelist);
                    obj.city = bookings.city;
                    obj.datetime = bookings.datetime;
                    obj.email = bookings.email;
                    obj.firstname = bookings.firstname;
                    obj.surname = bookings.surname;
                    obj.isedit = bookings.isedit;
                    obj.zipcode = bookings.zipcode;
                    obj.phone = bookings.phone;
                    obj.newsletter = bookings.newsletter;
                    obj.message = bookings.message;
                    obj.message = bookings.status;
                    obj.personal = bookings.personal;
                    obj.billingaddress = bookings.billingaddress;
                    obj.status = bookings.status;
                    var bookingid = _userService.savebookingdata(obj);
                    wp_glamly_payment payment = new wp_glamly_payment();
                    payment.acquirer = bookings.payment.acquirer;
                    payment.amount = bookings.payment.amount;
                    payment.approvalcode = bookings.payment.approvalcode;
                    payment.bookingid = bookingid;
                    payment.calcfee = bookings.payment.calcfee;
                    payment.cardexpdate = bookings.payment.cardexpdate;
                    payment.cardnomask = bookings.payment.cardnomask;
                    payment.cardprefix = bookings.payment.cardprefix;
                    payment.cardtype = bookings.payment.cardtype;
                    payment.currency = bookings.payment.cardtype;
                    payment.dibsInternalIdentifier = bookings.payment.dibsInternalIdentifier;
                    payment.fee = Convert.ToString(bookings.payment.fee);
                    payment.fullreply = bookings.payment.fullreply;
                    payment.lang = bookings.payment.lang;
                    payment.merchant = bookings.payment.merchant;
                    payment.merchantid = Convert.ToString(bookings.payment.merchantid);
                    payment.method = bookings.payment.method;
                    payment.mobilelib = bookings.payment.mobilelib;
                    payment.orderid = bookings.payment.orderid;
                    payment.paytype = bookings.payment.paytype;
                    payment.platform = bookings.payment.platform;
                    payment.status = bookings.payment.status;
                    payment.test = bookings.payment.test;
                    payment.textreply = bookings.payment.textreply;
                    payment.theme = bookings.payment.theme;
                    payment.timeout = bookings.payment.timeout;
                    payment.transact = bookings.payment.transact;
                    payment.userid = bookings.payment.userid;
                    payment.version = bookings.payment.version;
                    var userid = _userService.savepaymentdata(payment);

                    if (userid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been successfully saved";
                    }
                    else
                    {
                        resp.ResponseMessage = "provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InternalServerError;
                    }
                }

                else
                {
                    resp.ResponseMessage = "provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }
            }
            catch (Exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
            }
            return resp;
        }

        /// <summary>
        /// update the booking by bookingid
        /// </summary>
        /// <param name="bookings"></param>
        /// <returns></returns>
        [Route("updatebookings")]
        [HttpPost]
        public ResponseExtended<System.Web.Mvc.JsonResult> UpdateBookings(Booking bookings)
        {
            string jsonservicelist = string.Empty;
            string jsonservices = string.Empty;
            string jsontypes = string.Empty;
            string jsongpaymentlist = string.Empty;
            jsonservices = serialize.Serialize(bookings.service);
            jsontypes = serialize.Serialize(bookings.type);
            jsonservicelist = JsonConvert.SerializeObject(bookings.servicesWithType);
            ResponseExtended<System.Web.Mvc.JsonResult> resp = new ResponseExtended<System.Web.Mvc.JsonResult>();
            wp_glamly_servicesbookings bookingdetail = GetBookingById(bookings.bookingid);

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
                    bookingdetail.servicesWithType = serialize.Serialize(jsonservicelist);
                    bookingdetail.city = bookings.city;
                    bookingdetail.datetime = bookings.datetime;
                    bookingdetail.email = bookings.email;
                    bookingdetail.firstname = bookings.firstname;
                    bookingdetail.surname = bookings.surname;
                    bookingdetail.isedit = bookings.isedit;
                    bookingdetail.zipcode = bookings.zipcode;
                    bookingdetail.phone = bookings.phone;
                    bookingdetail.newsletter = bookings.newsletter;
                    bookingdetail.message = bookings.message;
                    bookingdetail.message = bookings.status;
                    bookingdetail.personal = bookings.personal;
                    bookingdetail.billingaddress = bookings.billingaddress;
                    bookingdetail.status = bookings.status;
                    bookingdetail.otherservices = bookingdetail.otherservices;
                    var bookingid = _userService.updatebookingdata(bookingdetail);
                    if (bookingid > 0)
                    {
                        resp.ResponseCode = Response.Codes.OK;
                        resp.ResponseMessage = "Booking has been updated successfully";
                    }
                    else
                    {
                        resp.ResponseMessage = "provided parameters are incorrect";
                        resp.ResponseCode = Response.Codes.InternalServerError;
                    }
                }
                else
                {
                    resp.ResponseMessage = "provided parameters are incorrect";
                    resp.ResponseCode = Response.Codes.InvalidRequest;
                }

            }
            catch (Exception)
            {
                resp.ResponseCode = Response.Codes.ApiUnauthorized;
            }
            return resp;
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
        public wp_glamly_servicesbookings GetBookingById(string id)
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

        /// <summary>
        /// Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Route("usersbookings/{id}")]
        [HttpGet]
        public List<wp_glamly_servicesbookings> GetBookingByUserId(int id)
        {
            try
            {
                return _userService.GetBookingByUserId(id);
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

        #region "Payment"
        /// <summary>
        /// Get payment recipt by userid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Route("payment/{id}")]
        [HttpGet]
        public List<wp_glamly_payment> GetPaymentById(int id)
        {
            try
            {
                return _userService.GetPaymentById(id);
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
        /// Get all services of stylists
        /// </summary>
        /// <returns></returns>
        [Route("payment")]
        [HttpGet]
        public List<wp_glamly_payment> GetPaymentList()
        {
            try
            {
                return _userService.GetPaymentList();
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
