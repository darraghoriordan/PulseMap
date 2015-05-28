﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class TradeMeEventServiceFake : ITradeMeEventService
    {
        private readonly GeoCoder _geocoder;
        readonly Random _rnd = new Random();

        public TradeMeEventServiceFake(GeoCoder geocoder)
        {
            _geocoder = geocoder;
        }

        public IEnumerable<TradeMeStandaloneEvent> GetLatestInterestingEvents()
        {
            var list = new List<TradeMeStandaloneEvent>();
            var selector = _rnd.Next(400, 500);
            for (var i = 0; i <= selector; i++)
            {
                list.Add(GetRandomEvent());
            }

            return list;
        }

        private readonly Dictionary<string, string> _addressDictionary = new Dictionary<string, string>()
        {
            { "Southland","Invercargill"},
              { "Canterbury","Christchurch City"},
                { "Auckland","Auckland City"},
                  { "Wellington","Wellington City"},
                    { "Marlborough","Blenheim"}
        };

        public TradeMeStandaloneEvent GetRandomEvent()
        {
            var now = DateTime.Now;
            var selector = _rnd.Next(0, _addressDictionary.Count);
            var tmEvent = new TradeMeStandaloneEvent
            {
                OccuredOn =
                    new DateTime(now.Year, now.Month, now.Day, now.Hour, RandomlyOffsetMinutes(now.Minute),
                        RandomlyOffsetSeconds()),
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

        public double RandomlyOffsetCoordinate(double number)
        {
            double minimum = number - .3;
            double maximum = number + .3;

            return _rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}