namespace Pulse.Services
{
    public class SettingsService
    {
        public static string GoogleGeoCodingApiUrl
        {
            get { return "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo"; }
        }

        public static string GoogleMapsUrl
        {
            get { return "https://maps.googleapis.com/maps/api/js?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo"; }
        }

        public static int OffsetInHours
        {
            get
            {
                return 5;
            }
        }

        
    }
}