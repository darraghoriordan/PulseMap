
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class GoogleApiClient : HttpClient, ICoordinateResolver
    {
        public GoogleApiClient()
        {
            this.BaseAddress = new Uri(SettingsService.GoogleGeoCodingApiUrl);
            this.DefaultRequestHeaders.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static string GetUrl(string region, string suburb)
        {
            var builder = new UriBuilder(SettingsService.GoogleGeoCodingApiUrl);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["address"] = region + "," + suburb;

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public static void ParseGoogleApiResponse(string googleGeoCodeResponseJson, GoogleGeoCodeResponse locality)
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
            catch (Exception)
            {
                //log and stop asking google until fixed
            }
        }

        public void ApplyCoordinatesToLocality(GoogleGeoCodeResponse locality)
        {
            //not async:)
            var result = this.GetAsync(GetUrl(locality.Region, locality.Suburb)).Result;
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            string content = result.Content.ReadAsStringAsync().Result;

           ParseGoogleApiResponse(content, locality);

        }

    }
}
