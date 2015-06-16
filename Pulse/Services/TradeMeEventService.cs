using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Dapper;
using Pulse.Models;

namespace Pulse.Services
{
    public class TradeMeEventService : ITradeMeEventService
    {
 
        private readonly GeoCoder geocoder;
        readonly Random rnd = new Random();
        private readonly IMapEventRepository tradeMeRepository = new MapEventRepository();

        public TradeMeEventService(GeoCoder geocoder)
        {
            this.geocoder = geocoder;
        }

        public IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents()
        {
            var listTmEvent = tradeMeRepository.GetSingleMapEvents();
            return listTmEvent.Select(GetStandaloneEvent).ToList();
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents()
        {
            var listTmEvent = tradeMeRepository.GetInteractionMapEvents();
            return listTmEvent.Select(GetInteractionEvent).ToList();
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents()
        {
            var listTmEvent = tradeMeRepository.GetComments();
            return listTmEvent.Select(GetInteractionEvent).ToList();
        }
        public int GetStatsSoldToday()
        {
            return tradeMeRepository.GetSoldToday();
        }

        public int GetStatsNewToday()
        {
            return tradeMeRepository.GetNewToday();
        }


        public TradeMeInteractionEvent GetInteractionEvent(TradeMeInteractionEvent myEvent)
        {

            myEvent.OccuredOn = myEvent.OccuredOn.AddHours(24 - SettingsService.OffsetInHours);

            geocoder.ApplyCoordinates(myEvent);

            myEvent.StartLatitude = RandomlyOffsetCoordinate(myEvent.StartLatitude);
            myEvent.StartLongitude = RandomlyOffsetCoordinate(myEvent.StartLongitude);

            myEvent.EndLatitude = RandomlyOffsetCoordinate(myEvent.EndLatitude);
            myEvent.EndLongitude = RandomlyOffsetCoordinate(myEvent.EndLongitude);

            return myEvent;
        }


        public TradeMeStandaloneEvent GetStandaloneEvent(TradeMeStandaloneEvent myEvent)
        {
            geocoder.ApplyCoordinates(myEvent);
            myEvent.OccuredOn = myEvent.OccuredOn.AddHours(24 - SettingsService.OffsetInHours);
            myEvent.Latitude = RandomlyOffsetCoordinate(myEvent.Latitude);
            myEvent.Longitude = RandomlyOffsetCoordinate(myEvent.Longitude);

            return myEvent;
        }

        public double RandomlyOffsetCoordinate(double number)
        {
            double minimum = number - .01;
            double maximum = number + .01;

            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }

    }
}