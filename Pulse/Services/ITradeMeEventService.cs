using System.Collections.Generic;
using Pulse.Models;

namespace Pulse.Services
{
    public interface ITradeMeEventService
    {
        IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents();
        IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents();
        IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents();
        int GetStatsSoldToday();
        int GetStatsNewToday();
    }
}