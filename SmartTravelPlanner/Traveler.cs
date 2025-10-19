using System;
using System.IO;
using System.Text.Json;

namespace Travelling
{
    public class Traveler : ICloneable
    {
        public string name { get; set; }
        public string currentLocation { get; set; } = "";
        public List<string> route { get; set; } = new List<string>();

        public Traveler() { }

        private static string CityName(string text)
        {
            if (text == null || text == "")
            {
                return "";
            }
            string result = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0 || text[i - 1] == ' ' || text[i - 1] == '-')
                {
                    result += char.ToUpper(text[i]);
                }
                else
                {
                    result += text[i];
                }
            }
            return result;
        }

        public Traveler(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetLocation(string location)
        {
            currentLocation = CityName(location);
        }

        public string GetLocation()
        {
            return currentLocation;
        }

        public void AddCity(string city)
        {
            if (city != null && city != "")
            {
                route.Add(CityName(city));
            }
            else
            {
                throw new ArgumentException("Invalid city!");
            }
        }

        public string GetRoute()
        {
            return string.Join(" -> ", route);
        }

        public override string ToString()
        {
            return $"Traveler: {name} | Location: {currentLocation} | Route: {GetRoute()}";
        }

        public void ClearRoute()
        {
            if (route != null)
            {
                route.Clear();
            }
        }

        public int GetStopCount()
        {
            return route.Count;
        }

        public bool HasCity(string city)
        {
            if (route.Contains(CityName(city)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SortRoute()
        {
            route.Sort();
        }

        public bool RemoveCity(string city)
        {
            if (route.Remove(CityName(city)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string? GetNextStop()
        {
            if (route.Count > 0)
            {
                return route[0];
            }
            else
            {
                return null;
            }
        }

        public string this[int index] { get => route[index]; }

        public static bool operator ==(Traveler a, Traveler b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return a.name == b.name && a.currentLocation == b.currentLocation;
        }

        public static bool operator !=(Traveler a, Traveler b)
        {
            return !(a == b);
        }

        public object Clone()
        {
            Traveler traveler_copy = new Traveler(name);
            traveler_copy.SetLocation(currentLocation);
            foreach (string city in route)
            {
                traveler_copy.AddCity(city);
            }
            return traveler_copy;
        }

        public void SaveToFile(string filePath)
        {
            string routeJson = JsonSerializer.Serialize(route, new JsonSerializerOptions { WriteIndented = false });
            string jsonString = "{\n" +
                                $"  \"name\": \"{name}\",\n" +
                                $"  \"currentLocation\": \"{currentLocation}\",\n" +
                                $"  \"route\": {routeJson}\n" +
                                "}";
            File.WriteAllText(filePath, jsonString);
        }
        public static Traveler LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File doesn’t exist");

            string jsonString = File.ReadAllText(path);

            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                string name = root.GetProperty("name").GetString() ?? "";
                string currentLocation = root.GetProperty("currentLocation").GetString() ?? "";
                Traveler traveler = new Traveler(name);
                traveler.SetLocation(currentLocation);

                if (root.TryGetProperty("route", out JsonElement routeElement) && routeElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement city in routeElement.EnumerateArray())
                    {
                        traveler.AddCity(city.GetString() ?? "");
                    }
                }

                return traveler;
            }
            catch (Exception)
            {
                throw new FileLoadException("Invalid travel data");
            }
        }

        public void PlanRouteTo(string destination, CityGraph map)
        {
            string start = "";
            if (currentLocation != null && currentLocation != "")
            {
                start = currentLocation;
            }
            else if (route.Count > 0)
            {
                start = route[0];
            }
            if (start == null)
            {
                Console.WriteLine("No route!");
                return;
            }

            List<string>? path = map.FindShortestPath(start, destination);

            if (path != null)
            {
                route.Clear();
                route.AddRange(path);
            }
        }
    }
}


