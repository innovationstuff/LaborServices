using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LaborServices.Web.Startup))]
namespace LaborServices.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
