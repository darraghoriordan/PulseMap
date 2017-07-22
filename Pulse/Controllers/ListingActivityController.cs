using Pulse.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Pulse.Controllers
{
    [RoutePrefix("api/events")]
    public class ListingActivityController : ApiController
    {
        private ITradeMeEventService _tradeMeEventService;
        public ListingActivityController(ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
        }

        [Route("standalone")]
        [HttpGet]
        public IEnumerable<Models.TradeMeStandaloneEvent> GetStandaloneEvents()
        {
            var events = _tradeMeEventService.GetLatestStandaloneEvents().OrderBy(x=> x.OccuredOn);
            return events;
        }

        [Route("interaction")]
        [HttpGet]
        public IEnumerable<Models.TradeMeInteractionEvent> GetInteractiveEvents()
        {
            var events = _tradeMeEventService.GetLatestInteractionEvents().OrderBy(x => x.OccuredOn);
            var s = events.Count();
            return events;
        }

        [Route("comments")]
        [HttpGet]
        public IEnumerable<Models.TradeMeInteractionEvent> GetCommentEvents()
        {
            var events = _tradeMeEventService.GetLatestCommentEvents().OrderBy(x => x.OccuredOn);
            var s = events.Count();
            return events;
        }
    }
}
