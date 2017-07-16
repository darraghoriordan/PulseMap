using System;

namespace Pulse.Services
{
    public interface ISettingsService
    {
        string GoogleGeoCodingApiUrl { get; }
        string GoogleMapsUrl { get; }
        int OffsetInHours { get; }
        TimeSpan ClientUpdateInterval { get; }
        TimeSpan EventStoreUpdateInterval { get; }
    }
}