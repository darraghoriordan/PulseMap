using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class GoogleApiCoordinateResolver : HttpClient, ICoordinateResolver
    {
        private readonly ISettingsService _settingsService;

        public GoogleApiCoordinateResolver(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            BaseAddress = new Uri(_settingsService.GoogleGeoCodingApiUrl);
            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string GetUrl(string region, string suburb)
        {
            var builder = new UriBuilder(_settingsService.GoogleGeoCodingApiUrl);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["address"] = region + "," + suburb;

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public void ParseGoogleApiResponse(string googleGeoCodeResponseJson, GoogleGeoCodeResponse locality)
        {
            try
            {
                double latitiude;
                double longitude;

                locality.FullResponse = googleGeoCodeResponseJson;

                dynamic googleGeoCodeResponse = JObject.Parse(googleGeoCodeResponseJson);

                double.TryParse(googleGeoCodeResponse.results[0].geometry.location.lat.ToString(), out latitiude);
                double.TryParse(googleGeoCodeResponse.results[0].geometry.location.lng.ToString(), out longitude);

                locality.Latitude = latitiude;
                locality.Longitude = longitude;

            }
            catch (Exception exception)
            {
                var message = exception.Message;
                Debug.WriteLine(message);
            }
        }

        public void ApplyCoordinatesToLocality(GoogleGeoCodeResponse locality)
        {
            //not async:)
            var result = GetAsync(GetUrl(locality.Region, locality.Suburb)).Result;
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            string content = result.Content.ReadAsStringAsync().Result;

            ParseGoogleApiResponse(content, locality);
        }
    }
}
