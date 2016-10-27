using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebChatSwitch.Web.Startup))]
namespace WebChatSwitch.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
