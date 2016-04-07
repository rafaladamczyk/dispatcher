using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
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
