﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using GlamlyData;
using GlamlyServices.Services;
using System.Text;
using System.Security.Cryptography;
using GlamlyData.Entities;
using GlamlyWebAPI.Library;
using System.Web.Script.Serialization;
using Conversive.PHPSerializationLibrary;
using Microsoft.Owin;

namespace GlamlyWebAPI.Providers
{
    /// <summary>
    /// Class to implement the owin authentication
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        Serializer serialize = new Serializer();
        JavaScriptSerializer javaserializer = new JavaScriptSerializer();
        private IUserServices _userService = new UserServices();
        string hashCode = "", encodingPasswordString = ""; int userid;
        /// <summary>
        /// method ValidateClientAuthentication validate the context of the entitydb
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// GrantResourceOwnerCredentials
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var user = new wp_users();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin == null) allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

           // IFormCollection parameters =  context.Request.ReadFormAsync();
            var form =  context.Request.ReadFormAsync();
            string usertypeid = form.Result.Get("usertype");
          //  var deviceId = form.Equals("usertype");

            //if (string.Equals(form["usertype"], "true", StringComparison.OrdinalIgnoreCase))
            //{
            //    // Add custom logic to handle the "remember me" case.
            //}
            // Validate your user and base on validation return claim identity or invalid_grant error
            //string email = context.UserName;
            //string password = context.Password;         
            bool isfacebook = false;
            string usertype = string.Empty;
            if (context.UserName != null)
            {
                var userdetail = _userService.GetUserByEmailId(context.UserName);
                if (userdetail != null)
                {
                    userid = Convert.ToInt32(userdetail.ID);

                    
                    if (!string.IsNullOrEmpty(context.Password))
                    {
                        hashCode = userdetail.user_activation_key;
                        encodingPasswordString = Hashing.MD5Hash(context.Password, hashCode);

                        var usermetadata = _userService.GetUserMetadatakeybyId(userid);
                        if (usermetadata != null)
                        {
                            var desearlize = serialize.Deserialize(usermetadata.meta_value);
                            UserData usercollection = javaserializer.Deserialize<UserData>(Convert.ToString(desearlize));
                            if (usercollection != null)
                            {
                                usertype = usercollection.user_type;
                            }
                        }
                        
                        if(usertypeid.Equals(usertypeid))
                        {
                            user = _userService.validationUser(context.UserName, encodingPasswordString);
                            isfacebook = _userService.IsFacebookLogin(Convert.ToInt32(userdetail.ID));
                        }
                      
                    }
                }
            }

            if (isfacebook || user != null)
            {
                var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                context.Validated(claimsIdentity);

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "Id", userid.ToString()
                    },
                });
                var ticket = new AuthenticationTicket(claimsIdentity, props);
                context.Validated(ticket);
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
            }
            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string encrypt(string message, string password)
        {
            byte[] results;
            UTF8Encoding utf8 = new UTF8Encoding();
            //to create the object for UTF8Encoding  class
            //TO create the object for MD5CryptoServiceProvider 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] deskey = md5.ComputeHash(utf8.GetBytes(password));
            //to convert to binary passkey
            //TO create the object for  TripleDESCryptoServiceProvider 
            TripleDESCryptoServiceProvider desalg = new TripleDESCryptoServiceProvider();
            desalg.Key = deskey;//to  pass encode key
            desalg.Mode = CipherMode.ECB;
            desalg.Padding = PaddingMode.PKCS7;
            byte[] encrypt_data = utf8.GetBytes(message);
            //to convert the string to utf encoding binary 

            try
            {
                //To transform the utf binary code to md5 encrypt    
                ICryptoTransform encryptor = desalg.CreateEncryptor();
                results = encryptor.TransformFinalBlock(encrypt_data, 0, encrypt_data.Length);
            }
            finally
            {
                //to clear the allocated memory
                desalg.Clear();
                md5.Clear();
            }
            //to convert to 64 bit string from converted md5 algorithm binary code
            return Convert.ToBase64String(results);

        }



        /// <summary>
        /// TokenEndpoint
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        //public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        //{
        //    // Resource owner password credentials does not provide a client ID.
        //    if (context.ClientId == null)
        //    {
        //        context.Validated();
        //    }

        //    return Task.FromResult<object>(null);
        //}

        //public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        //{
        //    if (context.ClientId == _publicClientId)
        //    {
        //        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        //        if (expectedRootUri.AbsoluteUri == context.RedirectUri)
        //        {
        //            context.Validated();
        //        }
        //    }

        //    return Task.FromResult<object>(null);
        //}
        /// <summary>
        /// CreateProperties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="useremail"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string id, string userName, string useremail)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", id },
                { "userName", userName },
                { "userEmail", useremail }
            };
            return new AuthenticationProperties(data);
        }
    }
}