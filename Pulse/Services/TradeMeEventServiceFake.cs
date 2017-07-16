using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class TradeMeEventServiceFake : ITradeMeEventService
    {
        private readonly IGeoCoder _geocoder;
        readonly Random _rnd = new Random();

        public TradeMeEventServiceFake(IGeoCoder geocoder)
        {
            _geocoder = geocoder;
        }

        public IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents()
        {
            var list = new List<TradeMeStandaloneEvent>();
            var selector = _rnd.Next(900, 1000);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomStandaloneEvent());
            }

            return list;
        }
        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents()
        {
            var list = new List<TradeMeInteractionEvent>();
            var selector = _rnd.Next(600, 800);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent());
            }

            return list;
        }
        
        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents()
        {
            var list = new List<TradeMeInteractionEvent>();
            var selector = _rnd.Next(600, 800);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent());
            }

            return list;
        }
        public int GetStatsSoldToday()
        {
            return 10000;
        }

        public int GetStatsNewToday()
        {
            return 10000;
        }

        private readonly Dictionary<string, string> _addressDictionary = new Dictionary<string, string>()
        {
            { "Southland","Invercargill"},
              { "Canterbury","Christchurch City"},
                { "Auckland","Auckland City"},
                  { "Wellington","Wellington City"},
                    { "Marlborough","Blenheim"}
        };
        public TradeMeInteractionEvent GetRandomInteractionEvent()
        {
            var now = DateTime.Now;
            var locationSelector1 = _rnd.Next(0, _addressDictionary.Count);
            var locationSelector2 = _rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeInteractionEvent()
            {
                OccuredOn =
                    new DateTime(now.Year, now.Month, now.Day, now.Hour, RandomlyOffsetMinutes(now.Minute),
                        RandomlyOffsetSeconds(), RandomlyOffsetMilliseconds()),
                StartRegion = _addressDictionary.ElementAt(locationSelector1).Key,
                StartSuburb = _addressDictionary.ElementAt(locationSelector1).Value,
                EndRegion  = _addressDictionary.ElementAt(locationSelector2).Key,
                EndSuburb = _addressDictionary.ElementAt(locationSelector2).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.StartLatitude = RandomlyOffsetCoordinate(tmEvent.StartLatitude);
            tmEvent.StartLongitude = RandomlyOffsetCoordinate(tmEvent.StartLongitude);

            tmEvent.EndLatitude = RandomlyOffsetCoordinate(tmEvent.EndLatitude);
            tmEvent.EndLongitude = RandomlyOffsetCoordinate(tmEvent.EndLongitude);

            return tmEvent;
        }
        public TradeMeStandaloneEvent GetRandomStandaloneEvent()
        {
            var now = DateTime.Now;
            var selector = _rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeStandaloneEvent
            {
                OccuredOn =
                    new DateTime(now.Year, now.Month, now.Day, now.Hour, RandomlyOffsetMinutes(now.Minute),
                        RandomlyOffsetSeconds(), RandomlyOffsetMilliseconds()),
                Region = _addressDictionary.ElementAt(selector).Key,
                Suburb = _addressDictionary.ElementAt(selector).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.Latitude = RandomlyOffsetCoordinate(tmEvent.Latitude);
            tmEvent.Longitude = RandomlyOffsetCoordinate(tmEvent.Longitude);

            return tmEvent;
        }

        public int RandomlyOffsetMinutes(int nowMinute)
        {
            int maximum = nowMinute;
            int minimum = nowMinute < 5 ? 0 : nowMinute - 5;

            return _rnd.Next(minimum, maximum);
        }

        public int RandomlyOffsetSeconds()
        {
            int maximum = 60;

            return _rnd.Next(maximum);
        }
        public int RandomlyOffsetMilliseconds()
        {
            int maximum = 999;

            return _rnd.Next(maximum);
        }
        public double RandomlyOffsetCoordinate(double number)
        {
            double minimum = number - .3;
            double maximum = number + .3;

            return _rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}