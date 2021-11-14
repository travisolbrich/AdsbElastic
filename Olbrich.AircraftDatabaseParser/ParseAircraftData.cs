using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Olbrich.AircraftDatabaseParser
{
    public class ParseAircraftData
    {
        public static AircraftIdentifier GetIdentifierData(string folder)
        {
            var files = Directory.GetFiles(folder, "*.json");
            var aircraftIdentities = new Dictionary<string, AircraftIdentity>();

            foreach (var file in files)
            {
                string json = File.ReadAllText(file);

                var obj = JsonConvert.DeserializeObject<Dictionary<string, Inner>>(json,
                    new JsonSerializerSettings()
                    {
                        Error = (sender, args) => { args.ErrorContext.Handled = true; } // lol who cares
                    });

                foreach (var (hex, value) in obj)
                {
                    var hexCodeBase = Path.GetFileNameWithoutExtension(file);
                    var hexCode = hexCodeBase + hex;
                    aircraftIdentities[hexCode] = new AircraftIdentity(hexCode, value.Registration, value.Type);
                }
            }

            return new AircraftIdentifier(aircraftIdentities);
        }

        class Inner
        {
            [JsonProperty("r")] public string Registration { get; set; }

            [JsonProperty("t")] public string Type { get; set; }
        }
    }
}