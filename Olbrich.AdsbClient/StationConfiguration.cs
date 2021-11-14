namespace Olbrich.AdsbClient
{
    public class StationConfiguration
    {
        public double StationLatitude { get; set; }
        public double StationLongitude { get; set; }
        public string ReceiverHostname { get; set; }
        public int ReceiverPort { get; set; }
        public string AircraftDbFolder { get; set; }
    }
}