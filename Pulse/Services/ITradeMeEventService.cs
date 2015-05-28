using System.Collections.Generic;
using Pulse.Models;

namespace Pulse.Services
{
    public interface ITradeMeEventService
    {
        IEnumerable<TradeMeStandaloneEvent> GetLatestInterestingEvents();
    }
}