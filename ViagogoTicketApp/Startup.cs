using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ViagogoTicketApp.Startup))]
namespace ViagogoTicketApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
