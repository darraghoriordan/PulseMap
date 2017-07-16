namespace Pulse.Services
{
    public class SettingsService : ISettingsService
    {
        public string GoogleGeoCodingApiUrl => "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo";

        public string GoogleMapsUrl => "https://maps.googleapis.com/maps/api/js?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo";

        public int OffsetInHours => 5;
    }
}