using Pulse.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Pulse.Controllers
{
    [RoutePrefix("api/events")]
    public class ListingActivityController : ApiController
    {
        private readonly ITradeMeEventService _tradeMeEventService;
        public ListingActivityController(ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
        }

        [Route("standalone")]
        [HttpGet]
        public IEnumerable<Models.TradeMeStandaloneEvent> GetStandaloneEvents(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetLatestStandaloneEvents(startDate, endDate).OrderBy(x=> x.OccuredOn);
            return events;
        }

        [Route("interaction")]
        [HttpGet]
        public IEnumerable<Models.TradeMeInteractionEvent> GetInteractiveEvents(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetLatestInteractionEvents(startDate, endDate).OrderBy(x => x.OccuredOn);
          
            return events;
        }

        [Route("comments")]
        [HttpGet]
        public IEnumerable<Models.TradeMeInteractionEvent> GetCommentEvents(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetLatestCommentEvents(startDate, endDate).OrderBy(x => x.OccuredOn);
         
            return events;
        }

        [Route("newdealergms")]
        [HttpGet]
        public IEnumerable<Models.StatModel> NewDealerGms(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetLatestStatsTotalDealerGms(startDate, endDate).OrderBy(x => x.OccuredOn);

            return events;
        }
    }
}
