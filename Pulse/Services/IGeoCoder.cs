using Pulse.Models;

namespace Pulse.Services
{
    public interface IGeoCoder
    {
        TradeMeStandaloneEvent ApplyCoordinates(TradeMeStandaloneEvent tmEvent);
        TradeMeInteractionEvent ApplyCoordinates(TradeMeInteractionEvent tmEvent);
    }
}