using NUnit.Framework;
using Pulse.Models;
using Pulse.Services;

namespace Tests
{
    [TestFixture]
    public class When_calling_google_geocoding
    {
        [Test]
        public void can_parse_json_response()
        {
            var coordinateResolver = new GoogleApiCoordinateResolver(new SettingsService());
            var stub = new GoogleGeoCodeResponse();
            var json = "{\"results\":[{\"address_components\":[{\"long_name\":\"1600\",\"short_name\":\"1600\",\"types\":[\"street_number\"]},{\"long_name\":\"Amphitheatre Pkwy\",\"short_name\":\"Amphitheatre Pkwy\",\"types\":[\"route\"]},{\"long_name\":\"Mountain View\",\"short_name\":\"Mountain View\",\"types\":[\"locality\",\"political\"]},{\"long_name\":\"Santa Clara County\",\"short_name\":\"Santa Clara County\",\"types\":[\"administrative_area_level_2\",\"political\"]},{\"long_name\":\"California\",\"short_name\":\"CA\",\"types\":[\"administrative_area_level_1\",\"political\"]},{\"long_name\":\"United States\",\"short_name\":\"US\",\"types\":[\"country\",\"political\"]},{\"long_name\":\"94043\",\"short_name\":\"94043\",\"types\":[\"postal_code\"]}],\"formatted_address\":\"1600 Amphitheatre Parkway, Mountain View, CA 94043, USA\",\"geometry\":{\"location\":{\"lat\":37.422478,\"lng\":-122.08425},\"location_type\":\"ROOFTOP\",\"viewport\":{\"northeast\":{\"lat\":37.423824,\"lng\":-122.0829},\"southwest\":{\"lat\":37.421127,\"lng\":-122.0856}}},\"place_id\":\"ChIJ2eUgeAK6j4ARbn5u_wAGqWA\",\"types\":[\"street_address\"]}],\"status\":\"OK\"}";

            coordinateResolver.ParseGoogleApiResponse(json, stub);

            Assert.That(stub.Latitude, Is.EqualTo(37.422478));
            Assert.That(stub.Longitude, Is.EqualTo(-122.08425));

        }
        [Test]
        public void can_get_valid_url()
        {
            var coordinateResolver = new GoogleApiCoordinateResolver(new SettingsService());
            var testUrl = coordinateResolver.GetUrl("test1", "test2");

            var isGood = testUrl == "http://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyDNL201u2ck7_67jdYM6L-Li2UAfFlQCHo&address=test1%2ctest2";
            Assert.That(isGood, Is.True);
        }
    }
}
