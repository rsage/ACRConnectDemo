using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestGoogle.Startup))]
namespace TestGoogle
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
