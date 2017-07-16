using System.Data.Entity;

namespace Pulse.Models
{
    public class ApplicationDbContext : DbContext, ILocalityDbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        /// <summary>
        /// overridden for interface
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public IDbSet<GoogleGeoCodeResponse> Localities { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}