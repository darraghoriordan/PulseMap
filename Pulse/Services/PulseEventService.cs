﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Pulse.Models;

namespace Pulse.Services
{
    /// <summary>
    /// this is going to be one big horrible controller for getting all the info every 'tick' we're interested in and updating any clients accordingly.
    /// </summary>
    public class PulseEventService
    {
       
        // Singleton instance
        // this also acts as a poor man's DI container :D
        private readonly static Lazy<PulseEventService> _instance = new Lazy<PulseEventService>(
            () => new PulseEventService(GlobalHost.ConnectionManager.GetHubContext<PulseSocketHub>().Clients, new TradeMeEventServiceFake(new GeoCoder(new GoogleApiClient(), ApplicationDbContext.Create()))));
        
        public static PulseEventService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private readonly ITradeMeEventService _tradeMeEventService;
        private readonly List<StandaloneEvent> _standaloneEvents;
        private readonly object _updateEventsLock = new object();
        private volatile bool _updatingEvents;
        private readonly TimeSpan _clientUpdateInterval = TimeSpan.FromMilliseconds(500);
        private readonly TimeSpan _trademeUpdateInterval = TimeSpan.FromMinutes(5);
        private DateTime _nextTradeMeUpdate;
        private DateTime _clientUpdateOffset;
        private Timer _updateEventsTimer;

        private PulseEventService(IHubConnectionContext<dynamic> clients, ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
            Clients = clients;
            _updateEventsTimer = new Timer(UpdateAllEvents, null, _clientUpdateInterval, _clientUpdateInterval);

            _standaloneEvents = new List<StandaloneEvent>();
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        private void UpdateAllEvents(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            // but we do everything in here so don't have to be tooooo careful
            lock (_updateEventsLock)
            {
                if (!_updatingEvents && UserManager.ConnectedIds.Any())
                {
                    _updatingEvents = true;

                    // should we update from trademe
                    if (_nextTradeMeUpdate <= DateTime.Now)
                    {
                        // clear old events and grab the last 5 mins of trademe activity
                        _standaloneEvents.Clear();
                        _standaloneEvents.AddRange(_tradeMeEventService.GetLatestInterestingEvents().Where(e => e.OccuredOn > DateTime.Now.Add(-_trademeUpdateInterval)));
                        // set the next update to be sometime after now
                        _nextTradeMeUpdate = DateTime.Now.AddMinutes(_trademeUpdateInterval.Minutes);

                    }
                    // update the offset time for displaying results to clients
                    _clientUpdateOffset = DateTime.Now.AddMinutes(-_trademeUpdateInterval.Minutes);
                    // find events that happened before this time
                    var events = _standaloneEvents.Where(e => e.OccuredOn < _clientUpdateOffset).ToList();

                    // remove the events so they're only sent to the client once.
                    _standaloneEvents.RemoveAll(e => e.OccuredOn < _clientUpdateOffset);

                    // send them if any exist
                    if (events.Any())
                    {
                        BroadcastStandaloneListingEvents(events);
                    }

                    _updatingEvents = false;
                }
            }
        }

        private void BroadcastStandaloneListingEvents(IEnumerable<StandaloneEvent> events)
        {
            Clients.All.updateStandaloneEvents(events);
        }

    }
}