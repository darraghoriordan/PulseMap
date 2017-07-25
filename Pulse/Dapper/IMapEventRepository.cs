using System.Collections.Generic;
using Pulse.Models;
using System;

namespace Pulse.Dapper
{
    public interface IMapEventRepository
    {
        IList<TradeMeStandaloneEvent> GetSingleMapEvents(DateTime startDate, DateTime endDate);
        IList<TradeMeInteractionEvent> GetInteractionMapEvents(DateTime startDate, DateTime endDate);
        IList<TradeMeInteractionEvent> GetComments(DateTime startDate, DateTime endDate);
        int GetSoldToday(DateTime startDate, DateTime endDate);
        int GetNewToday(DateTime startDate, DateTime endDate);
        int GetDealerGmsToday(DateTime startDate, DateTime endDate);
        IList<DealerGmsStatModel> GetLatestTotalDealerGms(DateTime startDate, DateTime endDate);
    }
}