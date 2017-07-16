using System.Runtime.InteropServices;
using Microsoft.AspNet.SignalR;
using Ninject.Modules;
using Pulse.Models;
using Pulse.Services;

namespace Pulse.NinjectModules
{
    public class MainNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<PulseSocketHub>().ToSelf();
            Bind<IPulseEventService>().To<PulseEventService>().InSingletonScope().WithConstructorArgument(
                "clients", GlobalHost.ConnectionManager.GetHubContext<PulseSocketHub>().Clients);
            Bind<ITradeMeEventService>().To<TradeMeEventService>();
            Bind<IGeoCoder>().To<GeoCoder>();
            Bind<ICoordinateResolver>().To<GoogleApiCoordinateResolver>();
            Bind<ISettingsService>().To<SettingsService>();
            Bind<IEventOffsetService>().To<EventOffsetService>();
            Bind<ILocalityStoreService>().To<LocalityStoreService>();
        }
    }
}