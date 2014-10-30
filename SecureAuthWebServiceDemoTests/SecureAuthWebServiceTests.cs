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

        [TestMethod]
        public void ReadProfileTest()
        {
            const string userName = "idtest1050";

            var client = new ProfileClient();

            var profile = client.ReadProfile(SvcLogin, SvcPassword, userName);

            Assert.IsNotNull(profile);

            foreach (var property in profile.Where(p=>p.Value != null && !string.IsNullOrEmpty(p.Value.ToString())))
            {
                Trace.TraceInformation("{0}={1}", property.Key, property.Value);
            }
        }

        [TestMethod]
        public void CreateUserAccountTest()
        {
            const string userName = "idtest1050";
            const string password = "B0okStores#";
            const string email = "srusinov@acr.org";
            const string firstName = "John";
            const string lastName = "Smith-" + userName;
            const string phone = "111-111-1111";
            const string institutionAddress = "Preston White Drive 101";
            const string personalAddress = "North Pole";
            const string institution = "ACR";
            const string department = "QA";
            const string jobTitle = "Head";

            var membershipClient = new MembershipClient();

            MembershipCreateStatus status;
            var user = membershipClient.CreateUser(SvcLogin, SvcPassword, userName, password, email, null, null, true, null, out status);

            Assert.IsNotNull(user);
            Assert.AreEqual(MembershipCreateStatus.Success, status);

            var profileClient = new ProfileClient();

            var properties = new[]
            {
                new UserProfileProperty { Key = "FirstName", Value = firstName}, 
                new UserProfileProperty { Key = "LastName", Value = lastName}, 
                new UserProfileProperty { Key = "Phone1", Value = phone}, 
                new UserProfileProperty { Key = "Email1", Value = email}, 
                new UserProfileProperty { Key = "AuxID5", Value = institutionAddress}, 
                new UserProfileProperty { Key = "AuxID6", Value = personalAddress}, 
                new UserProfileProperty { Key = "AuxID8", Value = institution}, 
                new UserProfileProperty { Key = "AuxID9", Value = department}, 
                new UserProfileProperty { Key = "AuxID10", Value = jobTitle},
                new UserProfileProperty { Key = "AuxID1", Value = "FALSE"},
            };

            var saved = profileClient.SaveProfile(SvcLogin, SvcPassword, userName, properties);

            Assert.IsTrue(saved);
        }

        [TestMethod]
        public void SaveProfileTest()
        {
            const string userName = "idtest1050";
            var client = new ProfileClient();

            var properties = new[]
            {
                //new UserProfileProperty { Key = "FirstName", Value = "John"}, 
                //new UserProfileProperty { Key = "LastName", Value = "Smith1020"}, 
                //new UserProfileProperty { Key = "Phone1", Value = "111-111-1111"}, 
                //new UserProfileProperty { Key = "Email1", Value = "srusinov@acr.org"}, 
                //new UserProfileProperty { Key = "StreetAddress", Value = "Preston White Drive 101"}, 
                //new UserProfileProperty { Key = "AuxID5", Value = "Preston White Drive"}, 
                //new UserProfileProperty { Key = "AuxID6", Value = "North Pole"}, 
                //new UserProfileProperty { Key = "AuxID8", Value = "ACR"}, 
                //new UserProfileProperty { Key = "AuxID9", Value = "QA"}, 
                //new UserProfileProperty { Key = "AuxID10", Value = "Head"},
                new UserProfileProperty { Key = "AuxID1", Value = "FALSE"},
            };

            var created = client.SaveProfile(SvcLogin, SvcPassword, userName, properties);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void CreateUserTest()
        {
            const string userName = "idtest1050";
            var client = new MembershipClient();

            MembershipCreateStatus status;
            var user = client.CreateUser(SvcLogin, SvcPassword, userName, "B0okStores#", "srusiniv@acr.org", "question", "answer", true, null, out status);

            Assert.IsNotNull(user);
            Assert.AreEqual(MembershipCreateStatus.Success, status);
        }

        [TestMethod]
        public void ChangePasswordTest()
        {
            const string userName = "idtest1050";
            var client = new MembershipClient();

            var result = client.ChangePassword(SvcLogin, SvcPassword, userName, "Passw0rd!", "B0okStores#");

            Assert.IsTrue(result);
        }
    }
}
