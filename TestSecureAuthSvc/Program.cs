using System;
using TestSecureAuthSvc.SecureAuthMembershipSvc;
using TestSecureAuthSvc.SecureAuthProfileSvc;

namespace TestSecureAuthSvc
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new MembershipClient();

            var user = proxy.GetUser("FBAService", "wh0Ar3Y06", "srusinov", false);

            Console.WriteLine("Membership.GetUser: userName={0}; email={1}", user.UserName, user.Email);

            var profileSvcProxy = new ProfileClient();

            var profile = profileSvcProxy.ReadProfile("FBAService", "wh0Ar3Y06", "srusinov");

            Console.WriteLine();
            Console.WriteLine("Profile.ReadProfile:");
            foreach (var profileProperty in profile)
            {
                if (profileProperty.Value == null || string.IsNullOrWhiteSpace(profileProperty.Value.ToString())) continue;
                Console.WriteLine("\t{0}={1}", profileProperty.Key, profileProperty.Value);
            }
        }
    }
}
