using System;

namespace Pulse.Models
{
    public class TradeMeStandaloneEvent : StandaloneEvent
    {
        public string Region { get; set; }
        public string Suburb { get; set; }
        public int CategoryId { get; set; }
    }
}