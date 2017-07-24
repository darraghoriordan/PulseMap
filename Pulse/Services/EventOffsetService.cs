using System;
using Pulse.Models;

namespace Pulse.Services
{
    public class EventOffsetService : IEventOffsetService
    {
        private readonly ISettingsService _settingsService;
        private readonly Random _rnd;

        public EventOffsetService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _rnd = new Random();
        }

        public TradeMeStandaloneEvent ApplyOffsets(TradeMeStandaloneEvent pulseEvent)
        {
            pulseEvent.OccuredOn = ApplyEventTimeOffset(pulseEvent.OccuredOn);
            pulseEvent.Latitude = ApplyRandomCoordinateOffset(pulseEvent.Latitude);
            pulseEvent.Longitude = ApplyRandomCoordinateOffset(pulseEvent.Longitude);
            return pulseEvent;
        }

        public TradeMeInteractionEvent ApplyOffsets(TradeMeInteractionEvent pulseEvent)
        {
            pulseEvent.OccuredOn = ApplyEventTimeOffset(pulseEvent.OccuredOn);
            pulseEvent.StartLatitude = ApplyRandomCoordinateOffset(pulseEvent.StartLatitude);
            pulseEvent.StartLongitude = ApplyRandomCoordinateOffset(pulseEvent.StartLongitude);

            pulseEvent.EndLatitude = ApplyRandomCoordinateOffset(pulseEvent.EndLatitude);
            pulseEvent.EndLongitude = ApplyRandomCoordinateOffset(pulseEvent.EndLongitude);

            return pulseEvent;
        }

        public DateTime ApplyEventTimeOffset(DateTime occuranceDate)
        {
            return occuranceDate.AddHours(24 - _settingsService.OffsetInHours);
        }
        public double ApplyRandomCoordinateOffset(double number)
        {
            double minimum = number - .01;
            double maximum = number + .01;

            return _rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}