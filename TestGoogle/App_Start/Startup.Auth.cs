using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json.Linq;
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

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

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
            var cert = new X509Certificate2(@"C:\Users\Сергей\Downloads\restonuat.cer");
            oidcConfiguration.SigningKeys.Add(new X509AsymmetricSecurityKey(cert));

            var oidOptions = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                Authority = "https://SecureAuth01VM.acr.org/secureauth1/",
                Caption = "ACR Connect",
                ClientId = "cdde8f20860c44e3b3e4d1833c7a5c54",
                ClientSecret = "f572d51d39324f3cadbe89c3ec5051db",
                RedirectUri = "https://localhost:44302/Account/ExternalLoginCallback",
                ResponseType = "id_token",
                ResponseMode = OpenIdConnectResponseModes.Query,
                Configuration = oidcConfiguration,
                Notifications = new OpenIdConnectAuthenticationNotifications {AuthorizationCodeReceived = GetUserProfile}
            };

            oidOptions.TokenValidationParameters.IssuerSigningKey = new X509AsymmetricSecurityKey(cert);
            oidOptions.ProtocolValidator.RequireNonce = false;
            oidOptions.TokenValidationParameters.SaveSigninToken = true;
            oidOptions.TokenValidationParameters.AuthenticationType = DefaultAuthenticationTypes.ExternalCookie;

            app.UseOpenIdConnectAuthentication(oidOptions);
        }

        private static async Task GetUserProfile(AuthorizationCodeReceivedNotification arg)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, arg.Options.Configuration.UserInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", arg.ProtocolMessage.AccessToken);
            var httpClient = new HttpClient();
            var graphResponse = await httpClient.SendAsync(request, arg.Request.CallCancelled);
            graphResponse.EnsureSuccessStatusCode();
            var text = await graphResponse.Content.ReadAsStringAsync();
            text = Cleanup(text);
            var user = JObject.Parse(text);

            var identity = arg.AuthenticationTicket.Identity;

            foreach (var val in user)
            {
                identity.AddClaim(new Claim(val.Key, val.Value.Value<string>(), ClaimValueTypes.String, arg.Options.AuthenticationType));
            }

            if (!identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.GetValue("sub").Value<string>(), ClaimValueTypes.String, arg.Options.AuthenticationType));
            }
        }

        private static string Cleanup(string text)
        {
            var bracketsCount = 0;
            var pos = 0;
            do
            {
                switch (text[pos])
                {
                    case '{':
                        bracketsCount++;
                        break;
                    case '}':
                        bracketsCount--;
                        break;
                }
                pos++;
            } while (bracketsCount > 0);

            return text.Substring(0, pos);
        }
    }
}