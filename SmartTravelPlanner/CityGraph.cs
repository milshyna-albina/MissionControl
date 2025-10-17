using System;
using System.IO;

namespace Travelling
{
    public class CityGraph
    {
        private Dictionary<string, List<(string ConnectedTo, int Distance)>> adjacencyList = new();
        private List<string> ordered = new();

        private CityGraph()
        {
        }

        public static CityGraph LoadFromFile(string filePath)
        {
            CityGraph graph = new CityGraph();

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(',');
                int distance = int.Parse(parts[1]);
                string[] cities = parts[0].Split('-');
                string cityA = cities[0];
                string cityB = cities[1];

                if (!graph.adjacencyList.ContainsKey(cityA))
                {
                    graph.adjacencyList[cityA] = new List<(string, int)>();
                }

                if (!graph.adjacencyList.ContainsKey(cityB))
                {
                    graph.adjacencyList[cityB] = new List<(string, int)>();
                }

                graph.adjacencyList[cityA].Add((cityB, distance));
                graph.adjacencyList[cityB].Add((cityA, distance));

                graph.ordered.Add($"{cityA}-{cityB},{distance}");
                graph.ordered.Add($"{cityB}-{cityA},{distance}");
            }

            return graph;
        }

        public override string ToString()
        {
            return string.Join("\n", ordered);
        }

        public List<string>? FindShortestPath(string from, string to)
        {
            if (!adjacencyList.ContainsKey(from) || !adjacencyList.ContainsKey(to))
                return null;

            var distances = adjacencyList.Keys.ToDictionary(city => city, city => int.MaxValue);
            var previous = adjacencyList.Keys.ToDictionary(city => city, city => (string?)null);
            distances[from] = 0;

            var queue = new PriorityQueue<string, int>();
            queue.Enqueue(from, 0);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();

                if (current == to)
                {
                    break;
                }

                int currentDistance = distances[current];

                foreach (var edge in adjacencyList[current])
                {
                    int distance = distances[edge.ConnectedTo];
                    int newDistance = currentDistance + edge.Distance;

                    if (newDistance < distance)
                    {
                        distances[edge.ConnectedTo] = newDistance;
                        previous[edge.ConnectedTo] = current;
                        queue.Enqueue(edge.ConnectedTo, newDistance);
                    }
                }
            }

            var path = new List<string>();
            string? currentStep = to;
            while (currentStep != null)
            {
                path.Add(currentStep);
                currentStep = previous[currentStep];
            }

            path.Reverse();
            return path[0] == from ? path : null;
        }

        public int GetPathDistance(List<string> path)
        {
            int totalDistance = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                string current = path[i];
                string next = path[i + 1];
                int distanceToNext = 0;

                foreach (var connection in adjacencyList[current])
                {
                    if (connection.ConnectedTo == next)
                    {
                        distanceToNext = connection.Distance;
                        break;
                    }
                }

                totalDistance += distanceToNext;
            }

            return totalDistance;
        }
    }
}
