﻿using Pulse.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Pulse.Controllers
{
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        private readonly ITradeMeEventService _tradeMeEventService;
        public StatisticsController(ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
        }

        [Route("newlistings")]
        [HttpGet]
        public int GetStatsNewToday(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetStatsNewToday(startDate, endDate);
            return events;
        }

        [Route("soldlistings")]
        [HttpGet]
        public int GetStatsSoldToday(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetStatsSoldToday(startDate, endDate);
            return events;
        }

        [Route("TotalDealerGms")]
        [HttpGet]
        public IEnumerable<Models.StatModel> GetStatsTotalDealerGms(DateTime startDate, DateTime endDate)
        {
            var events = _tradeMeEventService.GetLatestStatsTotalDealerGms(startDate,endDate).OrderBy(x => x.OccuredOn);
            return events;
        }
    }
}
