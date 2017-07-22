using Pulse.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IEnumerable<Models.TradeMeStandaloneEvent> GetEvents()
        {
            var events = _tradeMeEventService.GetLatestStandaloneEvents();
            return events;
        }
    }
}
