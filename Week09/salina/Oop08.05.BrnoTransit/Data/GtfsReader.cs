using System.IO.Compression;
using System.Text;
using BrnoTransit.Models;

namespace BrnoTransit.Data
{
    // turns the GTFS timetable into direct rides between neighbouring stops
    // GTFS = a zip of CSV files: stops.txt, routes.txt, trips.txt, stop_times.txt
    public static class GtfsReader
    {
        public const int Tram = 0;
        public const int Bus = 3;
        public const int Trolleybus = 11;

        public static List<Connection> ReadConnections(string zipPath, params int[] routeTypes)
        {
            using (ZipArchive zip = ZipFile.OpenRead(zipPath))
            {
                Dictionary<string, string> stopNames = ReadStops(zip);
                Dictionary<string, string> lines = ReadRoutes(zip, routeTypes);
                Dictionary<string, string> tripLines = ReadTrips(zip, lines);
                Dictionary<string, List<StopTime>> trips = ReadStopTimes(zip, tripLines);
                return Combine(trips, tripLines, stopNames);
            }
        }

        // stop_id -> stop name; the platforms of one stop share the name
        private static Dictionary<string, string> ReadStops(ZipArchive zip)
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            using (StreamReader reader = Open(zip, "stops.txt"))
            {
                Dictionary<string, int> column = Header(reader);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> field = Split(line);
                    names[field[column["stop_id"]]] = field[column["stop_name"]].Trim();
                }
            }
            return names;
        }

        // route_id -> line name such as "tram 4", only for the wanted kinds of vehicles
        private static Dictionary<string, string> ReadRoutes(ZipArchive zip, int[] routeTypes)
        {
            Dictionary<string, string> lines = new Dictionary<string, string>();
            using (StreamReader reader = Open(zip, "routes.txt"))
            {
                Dictionary<string, int> column = Header(reader);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> field = Split(line);
                    int type = int.Parse(field[column["route_type"]]);
                    if (Array.IndexOf(routeTypes, type) >= 0)
                    {
                        lines[field[column["route_id"]]] = TypeName(type) + " " + field[column["route_short_name"]].Trim();
                    }
                }
            }
            return lines;
        }

        // trip_id -> line name, only for trips of the wanted lines
        private static Dictionary<string, string> ReadTrips(ZipArchive zip, Dictionary<string, string> lines)
        {
            Dictionary<string, string> tripLines = new Dictionary<string, string>();
            using (StreamReader reader = Open(zip, "trips.txt"))
            {
                Dictionary<string, int> column = Header(reader);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> field = Split(line);
                    string lineName;
                    if (lines.TryGetValue(field[column["route_id"]], out lineName))
                    {
                        tripLines[field[column["trip_id"]]] = lineName;
                    }
                }
            }
            return tripLines;
        }

        // the stops of every wanted trip with their times
        private static Dictionary<string, List<StopTime>> ReadStopTimes(ZipArchive zip, Dictionary<string, string> tripLines)
        {
            Dictionary<string, List<StopTime>> trips = new Dictionary<string, List<StopTime>>();
            using (StreamReader reader = Open(zip, "stop_times.txt"))
            {
                Dictionary<string, int> column = Header(reader);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> field = Split(line);
                    string tripId = field[column["trip_id"]];
                    if (!tripLines.ContainsKey(tripId))
                    {
                        continue;
                    }
                    List<StopTime> stops;
                    if (!trips.TryGetValue(tripId, out stops))
                    {
                        stops = new List<StopTime>();
                        trips[tripId] = stops;
                    }
                    stops.Add(new StopTime(int.Parse(field[column["stop_sequence"]]), field[column["stop_id"]],
                                           Seconds(field[column["arrival_time"]]), Seconds(field[column["departure_time"]])));
                }
            }
            return trips;
        }

        // neighbouring stops of each trip become rides; the same ride on many trips is averaged
        private static List<Connection> Combine(Dictionary<string, List<StopTime>> trips,
                                                Dictionary<string, string> tripLines,
                                                Dictionary<string, string> stopNames)
        {
            Dictionary<string, double> totalMinutes = new Dictionary<string, double>();
            Dictionary<string, int> count = new Dictionary<string, int>();
            foreach (KeyValuePair<string, List<StopTime>> trip in trips)
            {
                List<StopTime> stops = trip.Value;
                stops.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));
                for (int i = 0; i + 1 < stops.Count; i++)
                {
                    string from = stopNames[stops[i].StopId];
                    string to = stopNames[stops[i + 1].StopId];
                    if (from == to)
                    {
                        continue;
                    }
                    double minutes = Math.Max(stops[i + 1].Arrival - stops[i].Departure, 30) / 60.0;
                    string key = from + "|" + to + "|" + tripLines[trip.Key];
                    if (!count.ContainsKey(key))
                    {
                        totalMinutes[key] = 0;
                        count[key] = 0;
                    }
                    totalMinutes[key] += minutes;
                    count[key]++;
                }
            }

            List<Connection> connections = new List<Connection>();
            foreach (KeyValuePair<string, int> item in count)
            {
                string[] parts = item.Key.Split('|');
                double average = Math.Round(totalMinutes[item.Key] / item.Value, 1);
                connections.Add(new Connection(parts[0], parts[1], parts[2], average, item.Value));
            }
            connections.Sort((a, b) =>
            {
                int byLine = string.CompareOrdinal(LineKey(a.Line), LineKey(b.Line));
                if (byLine != 0)
                {
                    return byLine;
                }
                return string.Compare(a.From, b.From, StringComparison.CurrentCulture);
            });
            return connections;
        }

        // "tram 4" -> "tram 0004", so that line 4 comes before line 12
        private static string LineKey(string line)
        {
            int space = line.IndexOf(' ');
            return line.Substring(0, space + 1) + line.Substring(space + 1).PadLeft(4, '0');
        }

        private record StopTime(int Sequence, string StopId, int Arrival, int Departure);

        private static string TypeName(int type)
        {
            if (type == Tram)
            {
                return "tram";
            }
            if (type == Trolleybus)
            {
                return "trolleybus";
            }
            return "bus";
        }

        // "25:07:30" -> seconds; GTFS times may go past midnight
        private static int Seconds(string time)
        {
            string[] parts = time.Trim().Split(':');
            return int.Parse(parts[0]) * 3600 + int.Parse(parts[1]) * 60 + int.Parse(parts[2]);
        }

        private static StreamReader Open(ZipArchive zip, string name)
        {
            ZipArchiveEntry entry = zip.GetEntry(name);
            if (entry == null)
            {
                throw new InvalidDataException(name + " is missing in the timetable");
            }
            return new StreamReader(entry.Open(), Encoding.UTF8);
        }

        // column name -> its position
        private static Dictionary<string, int> Header(StreamReader reader)
        {
            Dictionary<string, int> column = new Dictionary<string, int>();
            List<string> names = Split(reader.ReadLine());
            for (int i = 0; i < names.Count; i++)
            {
                column[names[i].Trim()] = i;
            }
            return column;
        }

        // one CSV line; a field in quotes may contain commas
        private static List<string> Split(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder field = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (quoted)
                {
                    if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else if (ch == '"')
                    {
                        quoted = false;
                    }
                    else
                    {
                        field.Append(ch);
                    }
                }
                else if (ch == '"')
                {
                    quoted = true;
                }
                else if (ch == ',')
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(ch);
                }
            }
            fields.Add(field.ToString());
            return fields;
        }
    }
}
