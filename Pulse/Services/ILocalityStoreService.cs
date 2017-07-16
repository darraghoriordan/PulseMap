using System.Collections.Generic;
using Pulse.Models;

namespace Pulse.Services
{
    public interface ILocalityStoreService
    {
        List<GoogleGeoCodeResponse> GetLocalities();
        void SaveLocality(GoogleGeoCodeResponse locality);
    }

   public  class LocalityStoreServiceFake : ILocalityStoreService
    {
        public List<GoogleGeoCodeResponse> GetLocalities()
        {
            return new List<GoogleGeoCodeResponse>();
        }

        public void SaveLocality(GoogleGeoCodeResponse locality)
        {
            // do nothing
        }
    }
}