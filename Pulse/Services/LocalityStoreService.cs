using System.Collections.Generic;
using System.Linq;
using Pulse.Models;

namespace Pulse.Services
{
    public class LocalityStoreService : ILocalityStoreService
    {
        private readonly ILocalityDbContext _localityDbContext;

        public LocalityStoreService()
        {
            _localityDbContext = ApplicationDbContext.Create();
        }

        public List<GoogleGeoCodeResponse> GetLocalities()
        {
            return _localityDbContext.Localities.ToList();
        }

        public void SaveLocality(GoogleGeoCodeResponse locality)
        {
            _localityDbContext.Localities.Add(locality);
            _localityDbContext.SaveChanges();
        }
    }
}