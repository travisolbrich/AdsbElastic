using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Geolocation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olbrich.AircraftDatabaseParser;

namespace Olbrich.AdsbClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<StationConfiguration> _stationConfiguration;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<StationConfiguration> stationConfiguration)
        {
            _logger = logger;
            _stationConfiguration = stationConfiguration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var identifier = ParseAircraftData.GetIdentifierData(_stationConfiguration.CurrentValue.AircraftDbFolder);
            
            Console.WriteLine($"Loaded {identifier.GetNumberOfRegistrations()} registrations.");
            
            var airplanes = new ConcurrentDictionary<string, TransmissionMessage>();

            Parallel.Invoke(() => ConsumeUpdates(stoppingToken, airplanes, _stationConfiguration.CurrentValue),
                async () => await PrintAircraftData(stoppingToken, airplanes, identifier, _stationConfiguration.CurrentValue));

            Console.WriteLine("Done.");
        }

        private static async Task PrintAircraftData(CancellationToken stoppingToken,
            IReadOnlyDictionary<string, TransmissionMessage> airplanes, AircraftIdentifier aircraftIdentifier, StationConfiguration stationConfiguration)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
                Console.WriteLine($"Current Aircraft ({airplanes.Count}):\n--------------------");
                foreach (var (k, v) in airplanes)
                {
                    var distance = v.Latitude != null && v.Longitude != null
                        ? GeoCalculator.GetDistance(stationConfiguration.StationLatitude, stationConfiguration.StationLongitude, (double) v.Latitude, (double) v.Longitude).ToString() + "mi"
                        : "";
                    
                    Console.WriteLine( $"[{v.HexIdent}] {v.Callsign} {aircraftIdentifier.GetIdentity(v.HexIdent)?.Type} at {v.Altitude}ft ({v.VerticalRate}fpm) {v.GroundSpeed}kt {v.Track}deg {v.Squawk} {distance}");
                }

                Console.WriteLine();
            }
        }

        private static Task ConsumeUpdates(CancellationToken stoppingToken, ConcurrentDictionary<string, TransmissionMessage> airplanes, StationConfiguration stationConfiguration)
        {
            var client = new TcpClient(stationConfiguration.ReceiverHostname, stationConfiguration.ReceiverPort);
            var stream = client.GetStream();

            var data = new Byte[512];
            string str = "";

            while (!stoppingToken.IsCancellationRequested)
            {
                // var responseBytes = await stream.ReadAsync(data, 0, data.Length, stoppingToken);
                stream.Read(data);
                str += Encoding.UTF8.GetString(data);

                var messages = str.Split("\r\n");

                var usableMessages = new List<string>();
                for (var i = 0; i < messages.Length; i++)
                {
                    var message = messages[i];
                    if (message.Count(x => x == ',') == 21)
                    {
                        usableMessages.Add(message);
                    }
                    else
                    {
                        if (i == messages.Length - 1)
                        {
                            // last message is incomplete . Append to next.
                            str = message;
                        }
                        else
                        {
                            // Last message is complete. Clear string.
                            str = "";
                        }
                    }
                }

                foreach (var transmission in usableMessages.Select(usableMessage =>
                    new TransmissionMessage(usableMessage)))
                {
                    if (airplanes.ContainsKey(transmission.HexIdent))
                    {
                        airplanes[transmission.HexIdent].Update(transmission);
                    }
                    else
                    {
                        airplanes[transmission.HexIdent] = transmission;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}