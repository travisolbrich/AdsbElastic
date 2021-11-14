using System;
using Geolocation;

namespace Olbrich.AdsbClient
{
    public class TransmissionMessage
    {
        public TransmissionMessage(string inputMessage)
        {
            var parts = inputMessage.Split(",");
            
            MessageType = parts[0];
            TransmissionType = parts[1];
            SessionId = parts[2];
            AircraftId = parts[3];
            HexIdent = parts[4];
            FlightId = parts[5];

            try
            {
                Callsign = parts[10].Trim();
                Altitude = parts[11] != "" ? double.Parse(parts[11]) : null;
                GroundSpeed = parts[12] != "" ? double.Parse(parts[12]) : null;
                Track = parts[13] != "" ? double.Parse(parts[13]) : null;
                Latitude = parts[14] != "" ? double.Parse(parts[14]) : null;
                Longitude = parts[15] != "" ? double.Parse(parts[15]) : null;
                VerticalRate = parts[16] != "" ? double.Parse(parts[16]) : null;
                Squawk = parts[17] != "" ? int.Parse(parts[17]) : null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not parse: {inputMessage}\n {e.Message}");
            }
        }

        public void Update(TransmissionMessage message)
        {
            if (!string.IsNullOrEmpty(message.Callsign)) Callsign = message.Callsign.Trim();
            if (message.Altitude != null) Altitude = message.Altitude;
            if (message.GroundSpeed != null) GroundSpeed = message.GroundSpeed;
            if (message.Track != null) Track = message.Track;
            if (message.Latitude != null) Latitude = message.Latitude;
            if (message.Longitude != null) Longitude = message.Longitude;
            if (message.VerticalRate != null) VerticalRate = message.VerticalRate;
            if (message.Squawk != null) Squawk = message.Squawk;
        }

        public string MessageType { get; }
        public string TransmissionType { get; }
        public string SessionId { get; }
        public string AircraftId { get; }
        public string HexIdent { get; }
        public string FlightId { get; }
        public DateTimeOffset GeneratedDate { get; }
        public DateTimeOffset LoggedTime { get; }
        public string Callsign { get; private set; }
        public double? Altitude { get; private set; }
        public double? GroundSpeed { get; private set; }
        public double? Track { get; private set; }
        public Coordinate location { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public double? VerticalRate { get; private set; }
        public int? Squawk { get; private set; }
        public bool? Alert { get; private set; }
        public bool? Emergency { get; private set; }
        public bool? Ident { get; private set; }
        public bool? IsOnGround { get; private set; }
    }
}