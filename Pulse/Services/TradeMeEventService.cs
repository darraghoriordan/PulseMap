﻿using System.Collections.Generic;
using System.Linq;
using Pulse.Dapper;
using Pulse.Models;
using System;

namespace Pulse.Services
{
    public class TradeMeEventService : ITradeMeEventService
    {
        private readonly IGeoCoder _geocoder;
        private readonly IMapEventRepository _mapEventRepository;
        private readonly IEventOffsetService _eventOffsetService;

        public TradeMeEventService(IGeoCoder geocoder, IMapEventRepository mapEventRepository, IEventOffsetService eventOffsetService)
        {
            _geocoder = geocoder;
            _mapEventRepository = mapEventRepository;
            _eventOffsetService = eventOffsetService;
        }

        public IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents(DateTime startDate, DateTime endDate)
        {
            var listTmEvent = _mapEventRepository.GetSingleMapEvents(startDate, endDate);
            return listTmEvent.Select(GetStandaloneEvent);
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents(DateTime startDate, DateTime endDate)
        {
            var listTmEvent = _mapEventRepository.GetInteractionMapEvents(startDate, endDate);
            return listTmEvent.Select(GetInteractionEvent);
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents(DateTime startDate, DateTime endDate)
        {
            var listTmEvent = _mapEventRepository.GetComments(startDate, endDate);
            return listTmEvent.Select(GetInteractionEvent);
        }
        public int GetStatsSoldToday(DateTime startDate, DateTime endDate)
        {
            return _mapEventRepository.GetSoldToday(startDate, endDate);
        }

        public int GetStatsNewToday(DateTime startDate, DateTime endDate)
        {
            return _mapEventRepository.GetNewToday(startDate, endDate);
        }
        public int GetDealerGmsToday(DateTime startDate, DateTime endDate)
        {
           var gmstoday = _mapEventRepository.GetDealerGmsToday(startDate, endDate);
            return gmstoday;
        }
        public IEnumerable<DealerGmsStatModel> GetLatestStatsTotalDealerGms(DateTime startDate, DateTime endDate)
        {
            var totalgms= _mapEventRepository.GetLatestTotalDealerGms(startDate, endDate);
            return  totalgms.Select(GetDealerGmsStatEvent);
        }
        public TradeMeInteractionEvent GetInteractionEvent(TradeMeInteractionEvent myEvent)
        {
            myEvent = _geocoder.ApplyCoordinates(myEvent);
            myEvent = _eventOffsetService.ApplyOffsets(myEvent);

            return myEvent;
        }

        public DealerGmsStatModel GetDealerGmsStatEvent(DealerGmsStatModel myEvent)
        {
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