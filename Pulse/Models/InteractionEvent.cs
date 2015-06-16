using System;

namespace Pulse.Models
{
    public class InteractionEvent 
    {
        public DateTime OccuredOn { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }

    }
}