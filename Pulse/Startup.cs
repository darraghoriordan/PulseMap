using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Pulse.Startup))]

namespace Pulse
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = false;
            hubConfiguration.EnableJavaScriptProxies = true;
            app.MapSignalR(hubConfiguration);
        }
    }
}
