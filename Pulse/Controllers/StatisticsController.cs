using Pulse.Services;
using System.Web.Http;

namespace Pulse.Controllers
{
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        private ITradeMeEventService _tradeMeEventService;
        public StatisticsController(ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
        }

        [Route("newlistings")]
        [HttpGet]
        public int GetStatsNewToday()
        {
            var events = _tradeMeEventService.GetStatsNewToday();
            return events;
        }

        [Route("soldlistings")]
        [HttpGet]
        public int GetStatsSoldToday()
        {
            var events = _tradeMeEventService.GetStatsSoldToday();
            return events;
        }
    }
}
