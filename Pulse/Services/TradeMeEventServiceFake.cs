using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class TradeMeEventServiceFake : ITradeMeEventService
    {
        private readonly IGeoCoder _geocoder;
        readonly Random _rnd = new Random(100);
        readonly Random _rnd2 = new Random(50);

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
                list.Add(GetRandomStandaloneEvent(_rnd));
            }

            return list;
        }

        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents()
        {
            var list = new List<TradeMeInteractionEvent>();
            var selector = _rnd.Next(400, 600);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent(_rnd));
            }

            return list;
        }
        
        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents()
        {
            var list = new List<TradeMeInteractionEvent>();
            var selector = _rnd2.Next(600, 800);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent(_rnd2));
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
        public TradeMeInteractionEvent GetRandomInteractionEvent(Random rnd)
        {
            var now = DateTime.Now;
            var locationSelector1 = rnd.Next(0, _addressDictionary.Count);
            var locationSelector2 = rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeInteractionEvent()
            {
                OccuredOn =
                    new DateTime(now.Year, now.Month, now.Day, now.Hour, RandomlyOffsetMinutes(now.Minute, rnd),
                        RandomlyOffsetSeconds(rnd), RandomlyOffsetMilliseconds(rnd)),
                StartRegion = _addressDictionary.ElementAt(locationSelector1).Key,
                StartSuburb = _addressDictionary.ElementAt(locationSelector1).Value,
                EndRegion  = _addressDictionary.ElementAt(locationSelector2).Key,
                EndSuburb = _addressDictionary.ElementAt(locationSelector2).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.StartLatitude = RandomlyOffsetCoordinate(tmEvent.StartLatitude, rnd);
            tmEvent.StartLongitude = RandomlyOffsetCoordinate(tmEvent.StartLongitude, rnd);

            tmEvent.EndLatitude = RandomlyOffsetCoordinate(tmEvent.EndLatitude, rnd);
            tmEvent.EndLongitude = RandomlyOffsetCoordinate(tmEvent.EndLongitude, rnd);

            return tmEvent;
        }
        public TradeMeStandaloneEvent GetRandomStandaloneEvent(Random rnd)
        {
            var now = DateTime.Now;
            var selector = rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeStandaloneEvent
            {
                OccuredOn =
                    new DateTime(now.Year, now.Month, now.Day, now.Hour, RandomlyOffsetMinutes(now.Minute, rnd),
                        RandomlyOffsetSeconds(rnd), RandomlyOffsetMilliseconds(rnd)),
                Region = _addressDictionary.ElementAt(selector).Key,
                Suburb = _addressDictionary.ElementAt(selector).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.Latitude = RandomlyOffsetCoordinate(tmEvent.Latitude, rnd);
            tmEvent.Longitude = RandomlyOffsetCoordinate(tmEvent.Longitude, rnd);

            return tmEvent;
        }

        public int RandomlyOffsetMinutes(int nowMinute, Random rnd)
        {
            int maximum = nowMinute;
            int minimum = nowMinute < 5 ? 0 : nowMinute - 5;

            return rnd.Next(minimum, maximum);
        }

        public int RandomlyOffsetSeconds(Random rnd)
        {
            int maximum = 60;

            return rnd.Next(maximum);
        }
        public int RandomlyOffsetMilliseconds(Random rnd)
        {
            int maximum = 999;

            return rnd.Next(maximum);
        }
        public double RandomlyOffsetCoordinate(double number, Random rnd)
        {
            double minimum = number - .3;
            double maximum = number + .3;

            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}