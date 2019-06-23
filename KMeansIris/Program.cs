using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KMeansIris.Hierarchical;

namespace KMeansIris
{
    public class Program
    {
        private static IrisData[] _irisData;
        private static double _2 = 0.01;

        public static void Main(string[] args)
        {
            _irisData = File.ReadAllLines("iris.csv")
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
                        })
                        .ToArray();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Expected:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var exSp = _irisData.GroupBy(x => x.Species);
            foreach (var g in exSp)
            {
                Console.WriteLine($"Species: {g.Key} [{g.Count()}]");
            }

            KMeansWithDistanceFunc(_irisData, DistanceFunctions.EuclideanDistance);
            KMeansWithDistanceFunc(_irisData, DistanceFunctions.ManhattanDistance);
            KMeansWithDistanceFunc(_irisData, DistanceFunctions.CosineDistance);
            KMeansWithDistanceFunc(_irisData, DistanceFunctions.ChebyshevDistance);

            AgnesClusteringPrint(_irisData, DistanceFunctions.CosineDistance, ClusterDistanceStrategy.CompleteLinkage);
            AgnesClusteringPrint(_irisData, DistanceFunctions.CosineDistance, ClusterDistanceStrategy.SingleLinkage);
            AgnesClusteringPrint(_irisData, DistanceFunctions.CosineDistance, ClusterDistanceStrategy.AverageWeightedPairGroupMethodArithmeticMean);
            AgnesClusteringPrint(_irisData, DistanceFunctions.CosineDistance, ClusterDistanceStrategy.AverageUnweightedPairGroupMethodArithmeticMean);


            AgnesClusteringPrint(_irisData, DistanceFunctions.EuclideanDistance, ClusterDistanceStrategy.CompleteLinkage);
            AgnesClusteringPrint(_irisData, DistanceFunctions.EuclideanDistance, ClusterDistanceStrategy.SingleLinkage);
            AgnesClusteringPrint(_irisData, DistanceFunctions.EuclideanDistance, ClusterDistanceStrategy.AverageWeightedPairGroupMethodArithmeticMean);
            AgnesClusteringPrint(_irisData, DistanceFunctions.EuclideanDistance, ClusterDistanceStrategy.AverageUnweightedPairGroupMethodArithmeticMean);

            Console.ReadLine();
        }

        private static void KMeansWithDistanceFunc(IrisData[] irisData, DistanceDelegate calculateDistanceFunction)
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
            PrintResult(bestResult.Clusters, irisData.Length, bestResult.Iteration);
        }

        private static void AgnesClusteringPrint(IrisData[] irisData, DistanceDelegate calculateDistanceFunction, ClusterDistanceStrategy strategy)
        {
            var irisDataHier = irisData.Select(d => new double[] { d.PetalLength, d.PetalWidth, d.SepalLength, d.SepalWidth }).ToHashSet();
            var hc = new AgnesClustering(calculateDistanceFunction, irisDataHier);
            var clusters = hc.Cluster(strategy, 3);

            var i = 0;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Agnes {calculateDistanceFunction.Method.Name} {strategy.ToString()} Predicted:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var res = new IrisData[clusters.Count()][];
            foreach (Cluster cl in clusters)
            {
                var pp = cl.GetAllPatterns();
                var clusterResult = pp.Select(p => p.GetAttributes()).Select(p => _irisData.First(x => 
                                                                                            x.PetalLength == p[0] &&
                                                                                            x.PetalWidth == p[1] &&
                                                                                            x.SepalLength == p[2] &&
                                                                                            x.SepalWidth == p[3])).ToArray();

                res[i++] = clusterResult;
            }

            PrintResult(res, irisData.Length);
            Console.WriteLine();
        }

        private static void PrintResult(IrisData[][] clusters, int setLength, int iteration = 0)
        {
            var correctCount = 0;
            for (int i = 0; i < clusters.GetLength(0); i++)
            {
                var cluster = clusters[i];
                var groups = cluster.GroupBy(x => x.Species)
                                    .Select(s => new {s.Key, Count = s.Count()})
                                    .OrderByDescending(s => s.Count)
                                    .ToList();
                correctCount += groups.FirstOrDefault()?.Count ?? 0;
                Console.Write($"Cluster {i}. "); 
                Console.WriteLine(string.Join(", ", groups.Select(s => $"{s.Key} [{s.Count}]").ToArray()));
            }

            if (iteration > 0)
            {
                Console.WriteLine($"Iteration: {iteration}");
            }

            var errorPercent = (1 - correctCount / (double)setLength) * 100;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Errors: {setLength - correctCount} [{Math.Round(errorPercent, 2)}%] [{correctCount}/{setLength}]");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}