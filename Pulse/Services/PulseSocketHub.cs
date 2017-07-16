using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Pulse.Services
{
    /// <summary>
    /// Stolen from microsofts starting signalR demos
    /// </summary>
    [HubName("pulseSocketHub")]
    public class PulseSocketHub : Hub
    {
        private readonly IPulseEventService _pulseEventService;
        
        public PulseSocketHub(IPulseEventService pulseEventService)
        {
            _pulseEventService = pulseEventService;
        }

        public override Task OnConnected()
        {
            UserManager.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserManager.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}