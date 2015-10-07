using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mélodie.Startup))]
namespace Mélodie
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
