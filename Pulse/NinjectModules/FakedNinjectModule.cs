using Ninject.Modules;
using Pulse.Services;

namespace Pulse.NinjectModules
{
    public class FakedNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Rebind<ITradeMeEventService>().To<TradeMeEventServiceFake>();
            Rebind<ICoordinateResolver>().To<GoogleApiCoordinateResolverFake>();
            Rebind<ILocalityStoreService>().To<LocalityStoreServiceFake>();
        }
    }
}