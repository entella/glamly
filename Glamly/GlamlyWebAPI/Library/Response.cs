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
            /// Invalid user authorization
            /// </summary>
            UserUnauthorized = 2,

            /// <summary>
            /// The provided parameters are incorrect.
            /// </summary>
            InvalidRequest = 3,


            /// <summary>
            /// Logon error: Wrong e-mail or password
            /// </summary>
            InvalidUser = 4,

            /// <summary>
            /// User email is already registered
            /// </summary>
            UserEmailExists = 5,

            /// <summary>
            /// Company no already registered
            /// </summary>
            CompanyExists = 6,
            /// <summary>
            /// Server Request
            /// </summary>
            Success = 200,
            /// <summary>
            /// Invalid API authorization
            /// </summary>
            ApiUnauthorized = 8,
            /// <summary>
            /// Invalid email address
            /// </summary>
            InvalidEmailAddress = 9,
            /// <summary>
            /// Already exist
            /// </summary>
            AlreadyExist = 7



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
                else if (ResponseCode == Codes.UserUnauthorized)
                    return "Invalid user authorization." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.UserEmailExists)
                    return "User already Exist." + ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
                else if (ResponseCode == Codes.InvalidRequest)
                    return ((!string.IsNullOrWhiteSpace(_message)) ? _message : "Request parameters are incorrect.");
                else
                    return ((!string.IsNullOrWhiteSpace(_message)) ? (" " + _message) : string.Empty);
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
