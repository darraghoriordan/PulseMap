using System.Data.Entity;

namespace Pulse.Models
{
    public interface ILocalityDbContext
    {
        IDbSet<GoogleGeoCodeResponse> Localities { get; set; }
        int SaveChanges();
    }
}