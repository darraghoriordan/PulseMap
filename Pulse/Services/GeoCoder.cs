using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class GeoCoder : IGeoCoder
    {
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly List<GoogleGeoCodeResponse> _coords;
        private readonly ILocalityStoreService _localityStoreService;

        public GeoCoder(ICoordinateResolver coordinateResolver, ILocalityStoreService localityStoreService)
        {
            _coordinateResolver = coordinateResolver;
            _localityStoreService = localityStoreService;
            _coords = new List<GoogleGeoCodeResponse>();
    
            //read localities from db to memory
            _coords.AddRange(_localityStoreService.GetLocalities());     
        }

        public TradeMeInteractionEvent ApplyCoordinates(TradeMeInteractionEvent tmEvent)
        {
            var startLocality = GetLocality(tmEvent.StartRegion, tmEvent.StartSuburb);
            tmEvent.StartLatitude = startLocality.Latitude;
            tmEvent.StartLongitude = startLocality.Longitude;

            var endLocality = GetLocality(tmEvent.EndRegion, tmEvent.EndSuburb);
            tmEvent.EndLatitude = endLocality.Latitude;
            tmEvent.EndLongitude = endLocality.Longitude;

            return tmEvent;
        }

        private GoogleGeoCodeResponse GetLocality(string region, string suburb)
        {
            //check local lookup for coords
            var locality =
                _coords.FirstOrDefault(l => l.Region == region && l.Suburb == suburb);

            if (locality == null)
            {
                //we need a new one, create a stub
                locality = new GoogleGeoCodeResponse()
                {
                    Region = region,
                    Suburb = suburb
                };

                // ask google for coords
                _coordinateResolver.ApplyCoordinatesToLocality(locality);

                //store it in the db and the local memory cache
                _localityStoreService.SaveLocality(locality);
                _coords.Add(locality);
            }
            return locality;
        }

        public TradeMeStandaloneEvent ApplyCoordinates(TradeMeStandaloneEvent tmEvent)
        {
            var locality = GetLocality(tmEvent.Region, tmEvent.Suburb);

            //apply coords to tm event model passed in
            tmEvent.Latitude = locality.Latitude;
            tmEvent.Longitude = locality.Longitude;

            return tmEvent;
        }
    }
}