using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebNdQn.Startup))]
namespace WebNdQn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
