using System.Data.Entity;

namespace Pulse.Models
{
    public class ApplicationDbContext : DbContext, ILocalityDbContext
    {
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
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
            return new ApplicationDbContext("LocalityConnection");
        }
    }
}