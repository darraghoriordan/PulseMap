using System;

namespace Pulse.Models
{
    public class StandaloneEvent : IMapCoordinates
    {
        public DateTime OccuredOn { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}