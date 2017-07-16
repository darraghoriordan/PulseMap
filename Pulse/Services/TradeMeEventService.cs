using System.Collections.Generic;
using System.Linq;
using Pulse.Dapper;
using Pulse.Models;

namespace Pulse.Services
{
    public class TradeMeEventService : ITradeMeEventService
    {
        private readonly GeoCoder _geocoder;
        private readonly IMapEventRepository _mapEventRepository;
        private readonly IEventOffsetService _eventOffsetService;

        public TradeMeEventService(GeoCoder geocoder, IMapEventRepository mapEventRepository, IEventOffsetService eventOffsetService)
        {
            _geocoder = geocoder;
            _mapEventRepository = mapEventRepository;
            _eventOffsetService = eventOffsetService;
        }

        public IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents()
        {
            var listTmEvent = _mapEventRepository.GetSingleMapEvents();
            return listTmEvent.Select(GetStandaloneEvent).ToList();
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents()
        {
            var listTmEvent = _mapEventRepository.GetInteractionMapEvents();
            return listTmEvent.Select(GetInteractionEvent).ToList();
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents()
        {
            var listTmEvent = _mapEventRepository.GetComments();
            return listTmEvent.Select(GetInteractionEvent).ToList();
        }
        public int GetStatsSoldToday()
        {
            return _mapEventRepository.GetSoldToday();
        }

        public int GetStatsNewToday()
        {
            return _mapEventRepository.GetNewToday();
        }

        public TradeMeInteractionEvent GetInteractionEvent(TradeMeInteractionEvent myEvent)
        {
            myEvent = _geocoder.ApplyCoordinates(myEvent);
            myEvent = _eventOffsetService.ApplyOffsets(myEvent);

            return myEvent;
        }

        public TradeMeStandaloneEvent GetStandaloneEvent(TradeMeStandaloneEvent myEvent)
        {
            myEvent = _geocoder.ApplyCoordinates(myEvent);
            myEvent = _eventOffsetService.ApplyOffsets(myEvent);

            return myEvent;
        }
    }
}