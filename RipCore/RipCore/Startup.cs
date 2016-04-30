using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RipCore.Startup))]
namespace RipCore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
