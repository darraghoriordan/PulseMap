using System;
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
        private readonly List<InteractionEvent> _interactionEvents;
        private readonly List<InteractionEvent> _commentEvents;
        private readonly object _updateEventsLock = new object();
        private volatile bool _updatingEvents;
        private readonly TimeSpan _clientUpdateInterval = TimeSpan.FromMilliseconds(400);
        private readonly TimeSpan _trademeUpdateInterval = TimeSpan.FromMinutes(5);
        private DateTime _nextTradeMeUpdate;
        private DateTime _clientUpdateOffset;
        private Timer _updateEventsTimer;
        private int _statsSoldToday;
        private int _statsNewToday;


        private PulseEventService(IHubConnectionContext<dynamic> clients, ITradeMeEventService tradeMeEventService)
        {
            _tradeMeEventService = tradeMeEventService;
            Clients = clients;
            _updateEventsTimer = new Timer(UpdateAllEvents, null, _clientUpdateInterval, _clientUpdateInterval);

            _standaloneEvents = new List<StandaloneEvent>();
            _interactionEvents = new List<InteractionEvent>();
            _commentEvents = new List<InteractionEvent>();
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

                        // set the next update to be sometime after now
                        _nextTradeMeUpdate = DateTime.Now.AddMinutes(_trademeUpdateInterval.Minutes);

                        // clear old events and grab the last 5 mins of trademe activity
                        _standaloneEvents.Clear();
                        _standaloneEvents.AddRange(_tradeMeEventService.GetLatestStandaloneEvents().Where(e => e.OccuredOn > DateTime.Now.Add(-_trademeUpdateInterval)));

                        // clear old events and grab the last 5 mins of trademe activity
                        _interactionEvents.Clear();
                        _interactionEvents.AddRange(_tradeMeEventService.GetLatestInteractionEvents().Where(e => e.OccuredOn > DateTime.Now.Add(-_trademeUpdateInterval)));
                        
                        // same for comments
                        _commentEvents.Clear();
                        _commentEvents.AddRange(_tradeMeEventService.GetLatestCommentEvents().Where(e => e.OccuredOn > DateTime.Now.Add(-_trademeUpdateInterval)));
                        
                        // refresh these every time to correct any possible 'drift' :)
                        _statsSoldToday = _tradeMeEventService.GetStatsSoldToday();
                        _statsNewToday = _tradeMeEventService.GetStatsNewToday();

                    }
                    // update the offset time for displaying results to clients
                    _clientUpdateOffset = DateTime.Now.AddMinutes(-_trademeUpdateInterval.Minutes);

                    // find events that happened before this time
                    var standaloneEventsToPush = _standaloneEvents.Where(e => e.OccuredOn < _clientUpdateOffset).ToList();
                    var interactionEventsToPush = _interactionEvents.Where(e => e.OccuredOn < _clientUpdateOffset).ToList();
                    var commentEventstoPush = _commentEvents.Where(e => e.OccuredOn < _clientUpdateOffset).ToList();

                    _statsSoldToday += interactionEventsToPush.Count();
                    _statsNewToday += standaloneEventsToPush.Count();
                    
                    //update these now
                    Clients.All.updateNewListingsStat(String.Format("{0:n0}", _statsNewToday));
                    Clients.All.updateSoldTodayStat(string.Format("{0:n0}", _statsSoldToday));
                    // remove the events so they're only sent to the client once.
                    _standaloneEvents.RemoveAll(e => e.OccuredOn < _clientUpdateOffset);
                    _interactionEvents.RemoveAll(e => e.OccuredOn < _clientUpdateOffset);
                    _commentEvents.RemoveAll(e => e.OccuredOn < _clientUpdateOffset);

                    // send them if any exist
                    if (standaloneEventsToPush.Any())
                    {
                        Clients.All.updateStandaloneEvents(standaloneEventsToPush);
                    }
                    if (interactionEventsToPush.Any())
                    {
                        Clients.All.updateInteractionEvents(interactionEventsToPush);
                    }
                    if (commentEventstoPush.Any())
                    {
                        Clients.All.updateCommentEvents(commentEventstoPush);
                    }

                    _updatingEvents = false;
                }
            }
        }
    }
}