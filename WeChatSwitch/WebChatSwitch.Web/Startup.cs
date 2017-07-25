using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeChatSwitch.Web.Startup))]
namespace WeChatSwitch.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
