using Newtonsoft.Json;

namespace GlamlyWebAPI.Library
{
    /// <summary>
    /// Class contains the owin token properties
    /// </summary>
    public class Token
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        [JsonProperty("access_token")]       
        public string AccessToken { get; set; }
        /// <summary>
        /// TokenType
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        /// <summary>
        /// ExpiresIn
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        /// <summary>
        /// RefreshToken
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }
        public int Id { get; set; }
    }
}