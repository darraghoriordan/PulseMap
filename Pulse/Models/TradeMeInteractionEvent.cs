using System;

namespace Pulse.Models
{
   public class TradeMeInteractionEvent : InteractionEvent
    {
        public string StartRegion { get; set; }
        public string StartSuburb { get; set; }
        public string EndRegion { get; set; }
        public string EndSuburb { get; set; }
        public int CategoryId { get; set; }
    }
}