namespace Olbrich.AircraftDatabaseParser
{
    public class AircraftIdentity
    {
        public AircraftIdentity(string hex, string tail, string type)
        {
            HexCode = hex;
            Tail = tail;
            Type = type;
        }

        public string HexCode { get; private set; }
        public string Tail { get; private set; }
        public string Type { get; private set; }
    }
}