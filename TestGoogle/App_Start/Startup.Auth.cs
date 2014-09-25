using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using TestGoogle.Models;

namespace TestGoogle
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "157308236840-o964nndk7unod1r0lkngbn4ap488hemr.apps.googleusercontent.com",
                ClientSecret = "T6R2a8yaOZvcN2ZuzZohdY6_"
            });

            var oidcConfiguration = new OpenIdConnectConfiguration
            {
                AuthorizationEndpoint = "https://SecureAuth01VM.acr.org/secureauth1/authorized/oidcauthorize.aspx",
                Issuer = "https://SecureAuth01VM.acr.org",
                TokenEndpoint = "https://SecureAuth01VM.acr.org/secureauth1/oidctoken.aspx",
                UserInfoEndpoint = "https://SecureAuth01VM.acr.org/secureauth1/oidcuserinfo.aspx",
            };
            oidcConfiguration.ResponseTypesSupported.Add("code");
            
            var cert = new X509Certificate2([Provide path to restonuat certificate here.]);
            oidcConfiguration.SigningKeys.Add(new X509AsymmetricSecurityKey(cert));

            var oidOptions = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                Authority = "https://SecureAuth01VM.acr.org/secureauth1/",
                Caption = "ACR Connect",
                ClientId = "cdde8f20860c44e3b3e4d1833c7a5c54",
                ClientSecret = "f572d51d39324f3cadbe89c3ec5051db",
                RedirectUri = "https://localhost:44302/Account/ExternalLoginCallback",
                ResponseType = "id_token",
                ResponseMode = OpenIdConnectResponseModes.Query,
                Configuration = oidcConfiguration,
            };

            oidOptions.TokenValidationParameters.IssuerSigningKey = new X509AsymmetricSecurityKey(cert);
            oidOptions.ProtocolValidator.RequireNonce = false;
            oidOptions.TokenValidationParameters.SaveSigninToken = true;
            oidOptions.TokenValidationParameters.AuthenticationType = DefaultAuthenticationTypes.ExternalCookie;

            app.UseOpenIdConnectAuthentication(oidOptions);
        }
    }
}