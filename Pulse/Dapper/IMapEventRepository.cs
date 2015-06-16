using System.Collections.Generic;
using Pulse.Models;

namespace Pulse.Dapper
{
    public interface IMapEventRepository
    {
        IList<TradeMeStandaloneEvent> GetSingleMapEvents();
        IList<TradeMeInteractionEvent> GetInteractionMapEvents();
        IList<TradeMeInteractionEvent> GetComments();
        int GetSoldToday();
        int GetNewToday();
    }
}