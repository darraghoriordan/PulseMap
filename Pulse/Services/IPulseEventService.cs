using Microsoft.AspNet.SignalR.Hubs;

namespace Pulse.Services
{
    public interface IPulseEventService
    {
        IHubConnectionContext<object> Clients { get; set; }
        void UpdateAllEvents(object state);
    }
}