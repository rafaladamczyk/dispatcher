using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Dispatcher.Startup))]

namespace Dispatcher
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
