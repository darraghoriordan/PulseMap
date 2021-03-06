﻿using Pulse.Models;

namespace Pulse.Services
{
    public class GoogleApiCoordinateResolverFake : ICoordinateResolver
    {
        public void ApplyCoordinatesToLocality(GoogleGeoCodeResponse locality)
        {  
            locality.FullResponse = "{\"results\":[{\"address_components\":[{\"long_name\":\"1600\",\"short_name\":\"1600\",\"types\":[\"street_number\"]},{\"long_name\":\"Amphitheatre Pkwy\",\"short_name\":\"Amphitheatre Pkwy\",\"types\":[\"route\"]},{\"long_name\":\"Mountain View\",\"short_name\":\"Mountain View\",\"types\":[\"locality\",\"political\"]},{\"long_name\":\"Santa Clara County\",\"short_name\":\"Santa Clara County\",\"types\":[\"administrative_area_level_2\",\"political\"]},{\"long_name\":\"California\",\"short_name\":\"CA\",\"types\":[\"administrative_area_level_1\",\"political\"]},{\"long_name\":\"United States\",\"short_name\":\"US\",\"types\":[\"country\",\"political\"]},{\"long_name\":\"94043\",\"short_name\":\"94043\",\"types\":[\"postal_code\"]}],\"formatted_address\":\"1600 Amphitheatre Parkway, Mountain View, CA 94043, USA\",\"geometry\":{\"location\":{\"lat\":37.422478,\"lng\":-122.08425},\"location_type\":\"ROOFTOP\",\"viewport\":{\"northeast\":{\"lat\":37.423824,\"lng\":-122.0829},\"southwest\":{\"lat\":37.421127,\"lng\":-122.0856}}},\"place_id\":\"ChIJ2eUgeAK6j4ARbn5u_wAGqWA\",\"types\":[\"street_address\"]}],\"status\":\"OK\"}";
            if (locality.Region == "Auckland") {
                locality.Latitude = -36.8406;
                locality.Longitude = 174.7400; }
            if (locality.Region == "Southland")
            {
                locality.Latitude = -46.4132;
                locality.Longitude = 168.3538;
            }
            if (locality.Region == "Canterbury")
            {
                locality.Latitude = -43.5321;
                locality.Longitude = 172.6362;
            }
            if (locality.Region == "Wellington")
            {
                locality.Latitude = -41.2865;
                locality.Longitude = 174.7762;
            }
            if (locality.Region == "Marlborough")
            {
                locality.Latitude = -41.2706;
                locality.Longitude = 173.2840;
            }
        }
    }
}