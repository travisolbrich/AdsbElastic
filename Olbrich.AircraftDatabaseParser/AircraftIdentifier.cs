using System.Collections.Generic;

namespace Olbrich.AircraftDatabaseParser
{
    public class AircraftIdentifier
    {
        private readonly IReadOnlyDictionary<string, AircraftIdentity> _aircraftIdentities;

        public AircraftIdentifier(IReadOnlyDictionary<string, AircraftIdentity> aircraftIdentities)
        {
            _aircraftIdentities = aircraftIdentities;
        }

        public AircraftIdentity? GetIdentity(string hexCode)
        {
            return _aircraftIdentities.ContainsKey(hexCode) ? _aircraftIdentities[hexCode] : null;
        }

        public int GetNumberOfRegistrations()
        {
            return _aircraftIdentities.Count;
        }
    }
}