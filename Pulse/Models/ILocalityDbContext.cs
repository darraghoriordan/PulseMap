using System.Data.Entity;

namespace Pulse.Models
{
    public interface ILocalityDbContext
    {
        DbSet<GoogleGeoCodeResponse> Localities { get; set; }
        int SaveChanges();
    }
}