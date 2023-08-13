using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgricultureApp.Startup))]
namespace AgricultureApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
