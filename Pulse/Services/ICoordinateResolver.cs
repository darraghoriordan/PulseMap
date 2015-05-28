using Pulse.Models;

namespace Pulse.Services
{
    public interface ICoordinateResolver
    {
        void ApplyCoordinatesToLocality(GoogleGeoCodeResponse locality);
    }
}