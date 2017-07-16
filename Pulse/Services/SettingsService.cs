using System;

namespace Pulse.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly TimeSpan _clientUpdateInterval = TimeSpan.FromMilliseconds(400);
        private readonly TimeSpan _eventStoreUpdateInterval = TimeSpan.FromSeconds(5);

        public string GoogleGeoCodingApiUrl => "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo";

        public string GoogleMapsUrl => "https://maps.googleapis.com/maps/api/js?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo";

        public int OffsetInHours => 5;

        public TimeSpan ClientUpdateInterval => _clientUpdateInterval;
        public TimeSpan EventStoreUpdateInterval => _eventStoreUpdateInterval;
    }
}