using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// Response class is response for http source code 
    /// </summary>
    public class Response
    {
        #region Enums
        /// <summary>
        /// Codes enum contains the various keys for authenticaton and authorization of the user
        /// </summary>
        public enum Codes : int
        {
            /// <summary>
            /// An error occured on the server
            /// </summary>
            InternalServerError = -1,

            /// <summary>
            /// Success
            /// </summary>
            OK = 1,
            /// <summary>
            /// Invalid API authorization
            /// </summary>
            ApiUnauthorized = 2,

            /// <summary>
            /// Invalid user authorization
            /// </summary>
            UserUnauthorized = 3,

            /// <summary>
            /// The request is missing or has empty parameters
            /// </summary>
            BadRequest = 4,

            /// <summary>
            /// The requested data could not be found
            /// </summary>
            NotFound = 5,

            /// <summary>
            /// The data exists
            /// </summary>
            Exists = 6,

            /// <summary>
            /// The provided parameters are incorrect.
            /// </summary>
            InvalidRequest = 7,

            /// <summary>
            /// Unable to send e-mail
            /// </summary>
            UnableToSendEmail = 8,
            /// <summary>
            /// User email is already registered
            /// </summary>
            UserEmailExists = 9,

            /// <summary>
            /// Logon error: Wrong e-mail or password
            /// </summary>
            InvalidUser = 10,

            /// <summary>
            /// User not found.
            /// </summary>
            UserNotFound = 11,

            /// <summary>
            /// Password and Confirm password doesn't match
            /// </summary>
            PasswordConfirmPasswordNotMatch = 12,
            /// <summary>
            /// Invalid email address
            /// </summary>
            InvalidEmailAddress = 13,

            /// <summary>
            /// Invalid password format
            /// </summary>
            InvalidPassword = 14,

            /// <summary>
            /// Forgot password success
            /// </summary>
            ForgotPasswordSuccess = 15,

            /// <summary>
            /// User deletion successful
            /// </summary>
            UserDeleteSuccess = 16,

            /// <summary>
            /// Failed Logout
            /// </summary>
            FailedLogout = 17,
            /// <summary>
            /// Failed password change
            /// </summary>
            Failed_PasswordChange = 18,

            /// <summary>
            /// Success password change
            /// </summary>
            Success_PasswordChange = 19,
            /// <summary>
            /// Invalid email or phone number
            /// </summary>
            InvalidEmailOrPhone = 20,

        }

        #endregion

        #region Properties
        /// <summary>
        /// Inner response status code
        /// </summary>
        public Codes ResponseCode { get; set; }

        private string _message = null;
        /// <summary>
        /// Inner response message
        /// </summary>
        public string ResponseMessage
        {
            get
            {
                if (ResponseCode == Codes.InternalServerError)
                    return "Internal Server Error." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.OK)
                    return "Success." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.ApiUnauthorized)
                    return "API Unauthorized." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserUnauthorized)
                    return "Invalid user authorization." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserEmailExists)
                    return "User already Exist." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.BadRequest)
                    return "The request is missing or empty parameters." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.NotFound)
                    return "Requested data not found." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.Exists)
                    return "User e-mail already exists." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidRequest)
                    return "Request parameters are incorrect." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UnableToSendEmail)
                    return "Unable to send e-mail." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserEmailExists)
                    return "UserEmail already Exist." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidUser)
                    return "Wrong username or password." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserNotFound)
                    return "User not found." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.PasswordConfirmPasswordNotMatch)
                    return "Password confirm does not match password." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidEmailAddress)
                    return "Invalid email address." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidPassword)
                    return "Invalid password format." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.ForgotPasswordSuccess)
                    return "Password is sent to your registered e-mail, check your inbox." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserDeleteSuccess)
                    return "User delete successfully." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.FailedLogout)
                    return "User has been logged out successfully." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.Failed_PasswordChange)
                    return "Password change failed." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.Success_PasswordChange)
                    return "Password has been changed successfully." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidEmailOrPhone)
                    return "User email and phone are invalid." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);              
                else if (ResponseCode == Codes.UserDeleteSuccess)
                    return "User delete successfully." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else
                    return "Incorrect Inputs." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
            }
            set
            {
                _message = value;
            }
        }

        #endregion

        #region Constructor
        public Response()
        {
            ResponseCode = Codes.InternalServerError;
        }

        public Response(Codes Code)
        {
            ResponseCode = Code;
        }
        #endregion
    }
    /// <summary>
    /// Response message with payload
    /// </summary>
    /// <typeparam name="T">Type of response payload</typeparam>
    public class ResponseExtended<T> : Response where T : class
    {
        #region Properties
        /// <summary>
        /// Response data payload
        /// </summary>
        public T ResponseData { get; set; }
        #endregion
    }
}
