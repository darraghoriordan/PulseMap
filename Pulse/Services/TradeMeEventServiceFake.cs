using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Models;
using System.Threading;

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

        public IEnumerable<TradeMeStandaloneEvent> GetLatestStandaloneEvents(DateTime startDate, DateTime endDate)
        {
            var list = new List<TradeMeStandaloneEvent>();
            int selector = GetSelector(_rnd, startDate, endDate);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomStandaloneEvent(_rnd, startDate, endDate));
            }

            return list;
        }

        private int GetSelector(Random rnd, DateTime startDate, DateTime endDate)
        {
            TimeSpan timeSpan = endDate - startDate;
            return rnd.Next((int)timeSpan.TotalSeconds, (int)(timeSpan.TotalSeconds * 2));
        }

        public IEnumerable<TradeMeInteractionEvent> GetLatestInteractionEvents(DateTime startDate, DateTime endDate)
        {
            var list = new List<TradeMeInteractionEvent>();
            int selector = GetSelector(_rnd, startDate, endDate);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent(_rnd, startDate, endDate));
            }

            return list;
        }

        public IEnumerable<TradeMeInteractionEvent> GetLatestCommentEvents(DateTime startDate, DateTime endDate)
        {
            var list = new List<TradeMeInteractionEvent>();
            int selector = GetSelector(_rnd2, startDate, endDate);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomInteractionEvent(_rnd2, startDate, endDate));
            }

            return list;
        }

        public int GetDealerGmsToday(DateTime startDate, DateTime endDate)
        {
            return 2000000;
        }

        public IEnumerable<StatModel> GetLatestStatsTotalDealerGms(DateTime startDate, DateTime endDate)
        {
            var list = new List<StatModel>();
            int selector = GetSelector(_rnd2, startDate, endDate);
            list.Add(GetRandomStatEvent(_rnd2, startDate, endDate));
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomStatEvent(_rnd2, startDate, endDate));
            }
            var statVal = 100000;
            foreach (var s in list.OrderBy(x => x.OccuredOn))
            {
                s.StartStat = statVal;
                statVal += _rnd2.Next(1, 1000);
            }

            return list;
        }



        public int GetStatsSoldToday(DateTime startDate, DateTime endDate)
        {
            return 10000;
        }

        public int GetStatsNewToday(DateTime startDate, DateTime endDate)
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

        protected DateTime GetRandomDate(Random rnd, DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            TimeSpan timeSpan = endDate - startDate;
            if (timeSpan.TotalSeconds <= 0)
            {
                throw new Exception("The end time must be greater than the start time.");
            }

            TimeSpan newSpan = new TimeSpan(0, 0, 0, rnd.Next(0, (int)timeSpan.TotalSeconds));
            DateTime newDate = startDate + newSpan;
            return newDate;
        }
        public TradeMeInteractionEvent GetRandomInteractionEvent(Random rnd, DateTime startDate, DateTime endDate)
        {

            var locationSelector1 = rnd.Next(0, _addressDictionary.Count);
            var locationSelector2 = rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeInteractionEvent()
            {
                OccuredOn = GetRandomDate(rnd, startDate, endDate),
                StartRegion = _addressDictionary.ElementAt(locationSelector1).Key,
                StartSuburb = _addressDictionary.ElementAt(locationSelector1).Value,
                EndRegion = _addressDictionary.ElementAt(locationSelector2).Key,
                EndSuburb = _addressDictionary.ElementAt(locationSelector2).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.StartLatitude = RandomlyOffsetCoordinate(tmEvent.StartLatitude, rnd);
            tmEvent.StartLongitude = RandomlyOffsetCoordinate(tmEvent.StartLongitude, rnd);

            tmEvent.EndLatitude = RandomlyOffsetCoordinate(tmEvent.EndLatitude, rnd);
            tmEvent.EndLongitude = RandomlyOffsetCoordinate(tmEvent.EndLongitude, rnd);

            return tmEvent;
        }
        public TradeMeStandaloneEvent GetRandomStandaloneEvent(Random rnd, DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            var selector = rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeStandaloneEvent
            {
                OccuredOn = GetRandomDate(rnd, startDate, endDate),
                Region = _addressDictionary.ElementAt(selector).Key,
                Suburb = _addressDictionary.ElementAt(selector).Value
            };

            _geocoder.ApplyCoordinates(tmEvent);

            tmEvent.Latitude = RandomlyOffsetCoordinate(tmEvent.Latitude, rnd);
            tmEvent.Longitude = RandomlyOffsetCoordinate(tmEvent.Longitude, rnd);

            return tmEvent;
        }
        private StatModel GetRandomStatEvent(Random rnd2, DateTime startDate, DateTime endDate)
        {
            var stat = new StatModel()
            {
                OccuredOn = GetRandomDate(rnd2, startDate, endDate)
            };

            return stat;
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