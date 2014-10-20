using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureAuthWebServiceDemoTests.MembershipService;
using SecureAuthWebServiceDemoTests.ProfileService;

namespace SecureAuthWebServiceDemoTests
{
    [TestClass]
    public class SecureAuthWebServiceTests
    {
        private const string SvcLogin = "FBAService";
        private const string SvcPassword = "wh0Ar3Y06";

        private const string UserLogin = "idtest1000";
        private const string NewUserLogin = "idtest1010";

        [TestMethod]
        public void ReadProfileTest()
        {
            var client = new ProfileClient();

            var profile = client.ReadProfile(SvcLogin, SvcPassword, UserLogin);

            Assert.IsNotNull(profile);

            foreach (var property in profile.Where(p=>p.Value != null && !string.IsNullOrEmpty(p.Value.ToString())))
            {
                Trace.TraceInformation("{0}={1}", property.Key, property.Value);
            }
        }

        [TestMethod]
        public void SaveProfileTest()
        {
            var client = new ProfileClient();

            var properties = new[]
            {
                new UserProfileProperty { Key = "FirstName", Value = "John"}, 
                new UserProfileProperty { Key = "LastName", Value = "Smith"}, 
                new UserProfileProperty { Key = "Phone1", Value = "111-111-1111"}, 
                new UserProfileProperty { Key = "Email1", Value = "srusinov@acr.org"}, 
                new UserProfileProperty { Key = "StreetAddress", Value = "Preston White Drive 101"}, 
                new UserProfileProperty { Key = "AuxID8", Value = "ACR"}, 
                new UserProfileProperty { Key = "AuxID9", Value = "QA"}, 
                new UserProfileProperty { Key = "AuxID10", Value = "Head"} 
            };

            var created = client.SaveProfile(SvcLogin, SvcPassword, UserLogin, properties);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var client = new MembershipClient();

            MembershipCreateStatus status;
            var user = client.CreateUser(SvcLogin, SvcPassword, NewUserLogin, "Passw0rd!", "srusiniv@acr.org", "question", "answer", true, null, out status);

            Assert.IsNotNull(user);
            Assert.AreEqual(NewUserLogin, user.UserName);
            Assert.AreEqual(MembershipCreateStatus.Success, status);
        }
    }
}
