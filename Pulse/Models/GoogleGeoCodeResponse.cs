using System.ComponentModel.DataAnnotations;

namespace Pulse.Models
{
    /// <summary>
    /// Adding now to remind about storing the whole google response!
    /// </summary>
    public class GoogleGeoCodeResponse
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string Suburb { get; set; }
        [Required]
        public string FullResponse { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}