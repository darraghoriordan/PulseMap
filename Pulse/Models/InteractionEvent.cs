using System;

namespace Pulse.Models
{
    public class InteractionEvent 
    {
        public DateTime OccuredOn { get; set; }
        public double StartLatitute { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitute { get; set; }
        public double EndLongitude { get; set; }
    }
}