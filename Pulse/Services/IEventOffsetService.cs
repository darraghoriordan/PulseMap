using System;
using Pulse.Models;

namespace Pulse.Services
{
    public interface IEventOffsetService
    {
        TradeMeStandaloneEvent ApplyOffsets(TradeMeStandaloneEvent pulseEvent);
        DateTime ApplyEventTimeOffset(DateTime occuranceDate);
        double ApplyRandomCoordinateOffset(double number);
        TradeMeInteractionEvent ApplyOffsets(TradeMeInteractionEvent pulseEvent);
        DealerGmsStatModel ApplyOffsets(DealerGmsStatModel pulseEvent);
    }
}