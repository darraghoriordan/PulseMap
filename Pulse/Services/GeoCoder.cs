using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class GeoCoder
    {
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly List<GoogleGeoCodeResponse> _coords;
        private readonly ILocalityDbContext _localityDbContext;

        public GeoCoder(ICoordinateResolver coordinateResolver, ILocalityDbContext localityDbContext)
        {
            _coordinateResolver = coordinateResolver;
            _localityDbContext = localityDbContext;
            _coords = new List<GoogleGeoCodeResponse>();
    
            //read localities from db to memory
            _coords.AddRange(_localityDbContext.Localities);     
        }

        private void StoreLocalityInPermmenantCache(GoogleGeoCodeResponse locality)
        {
            _localityDbContext.Localities.Add(locality);
            _localityDbContext.SaveChanges();
        }

        public void ApplyCoordinates(TradeMeStandaloneEvent tmEvent)
        {
            //check local lookup for coords
            var locality =
                _coords.FirstOrDefault(l => l.Region == tmEvent.Region && l.Suburb == tmEvent.Suburb);

            if (locality == null)
            {
                //we need a new one, create a stub
                locality = new GoogleGeoCodeResponse()
                {
                    Region = tmEvent.Region,
                    Suburb = tmEvent.Suburb
                };

                // ask google for coords
                _coordinateResolver.ApplyCoordinatesToLocality(locality);
               
                //store it in the db and the local memory cache
                StoreLocalityInPermmenantCache(locality);
                _coords.Add(locality);
            }

            //apply coords to tm event model passed in
            tmEvent.Latitude = locality.Latitude;
            tmEvent.Longitude = locality.Longitude;

        }
    }
}