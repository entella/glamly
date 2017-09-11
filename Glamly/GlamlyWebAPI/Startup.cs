using System.Threading.Tasks;
using GlamlyWebAPI.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(GlamlyWebAPI.Startup))]

namespace GlamlyWebAPI
{
    /// <summary>
    /// Startup class of the application
    /// </summary>
    public partial class Startup
    {

        /// <summary>
        /// OAuthBearerOptions variable
        /// </summary>
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        //  public static string PublicClientId { get; private set; }
        /// <summary>
        /// method to generate the token with path /AuthenticationToken
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
        /// <summary>
        /// Configure the token path
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            // Configure the application for OAuth based flow
            //  Startup.PublicClientId = "self";
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/AuthenticationToken"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new ApplicationOAuthProvider(),
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                AuthenticationType = "Bearer"
            });

        }
    }
}