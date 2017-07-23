using System.Collections.Generic;
using Pulse.Models;
using System;

namespace Pulse.Services
{
    public interface ITradeMeEventService
    {
        IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents(DateTime startDate, DateTime endDate);
        IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents(DateTime startDate, DateTime endDate);
        IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents(DateTime startDate, DateTime endDate);
        int GetStatsSoldToday(DateTime startDate, DateTime endDate);
        int GetStatsNewToday(DateTime startDate, DateTime endDate);
        IEnumerable<StatModel> GetLatestStatsTotalDealerGms(DateTime startDate, DateTime endDate);
    }
}