using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KMeansIris
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var irisData = File.ReadAllLines("iris.csv")
                        .Skip(1) // header
                        .Select(v => v.Split(','))
                        .Select(s => new IrisData
                        {
                            Id = s[0], 
                            SepalLength = float.Parse(s[1]), 
                            SepalWidth = float.Parse(s[2]),
                            PetalLength = float.Parse(s[3]),
                            PetalWidth = float.Parse(s[4]),
                            Species = s[5]
                        } )
                        .ToArray();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Expected:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var exSp = irisData.GroupBy(x => x.Species);
            foreach (var g in exSp)
            {
                Console.WriteLine($"Species: {g.Key} [{g.Count()}]");
            }

            KMeansWithDistanceFunc(irisData, DistanceFunctions.EuclideanDistance);
            KMeansWithDistanceFunc(irisData, DistanceFunctions.ManhattanDistance);
            KMeansWithDistanceFunc(irisData, DistanceFunctions.CosineDistance);
            KMeansWithDistanceFunc(irisData, DistanceFunctions.ChebyshevDistance);
            Console.ReadLine();
        }

        private static void KMeansWithDistanceFunc(IrisData[] irisData, KMeansCalculateDistanceDelegate calculateDistanceFunction)
        {
            Console.WriteLine();
            var results = new List<KMeans.Result<IrisData>>(25);
            for (int i = 0; i < results.Capacity; i++)
            {
                var result = KMeans.Cluster(calculateDistanceFunction, irisData, 3);
                results.Add(result);
            }

            var bestResult = results.Aggregate((minItem, nextItem) => minItem.TotalDistance < nextItem.TotalDistance ? minItem : nextItem);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"KMeans {calculateDistanceFunction.Method.Name} Predicted:");
            Console.ForegroundColor = ConsoleColor.Gray;
            PrintResult(bestResult, irisData.Length);
        }

        private static void PrintResult(KMeans.Result<IrisData> result, int setLength)
        {
            var correctCount = 0;
            for (int i = 0; i < result.Clusters.GetLength(0); i++)
            {
                var cluster = result.Clusters[i];
                var groups = cluster.GroupBy(x => x.Species)
                                    .Select(s => new {s.Key, Count = s.Count()})
                                    .OrderByDescending(s => s.Count)
                                    .ToList();
                correctCount += groups.FirstOrDefault()?.Count ?? 0;
                Console.Write($"Cluster {i}. "); 
                Console.WriteLine(string.Join(", ", groups.Select(s => $"{s.Key} [{s.Count}]").ToArray()));
            }

            var errorPercent = (1 - correctCount / (double)setLength) * 100;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Errors: {setLength - correctCount} [{Math.Round(errorPercent, 2)}%] [{correctCount}/{setLength}]");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}