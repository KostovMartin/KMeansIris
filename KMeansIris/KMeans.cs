using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KMeansIris
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KMeansValueAttribute : Attribute 
    { 
    }

    public delegate double KMeansCalculateDistanceDelegate(double[] point, double[] centroid);

    public static class KMeans
    {
        public static Result<T> Cluster<T>(KMeansCalculateDistanceDelegate calculateDistanceFunction, T[] items, int clusterCount, int maxIterations = 100)
        {
            var data = ConvertToVectors(items);

            var hasChanges = true;
            var iteration = 0;
            double totalDistance = 0;
            var numData = data.Length;

            // Create a random initial clustering assignment
            var clustering = InitializeClustering(numData, clusterCount);

            var centroidIdx = new int[clusterCount];
            var clusterItemCount = new int[clusterCount];

            // Perform the clustering
            while (hasChanges && iteration < maxIterations)
            {
                clusterItemCount = new int[clusterCount];
                totalDistance = CalculateClusteringInformation(data, clustering, ref centroidIdx, clusterCount, ref clusterItemCount, calculateDistanceFunction);
                hasChanges = AssignClustering(data, clustering, centroidIdx, clusterCount, calculateDistanceFunction);
                ++iteration;
            }

            var clusters = new T[clusterCount][];
            for (var k = 0; k < clusters.Length; k++)
            {
                clusters[k] = new T[clusterItemCount[k]];
            }

            var clustersCurIdx = new int[clusterCount];
            for (var i = 0; i < clustering.Length; i++)
            {
                var c = clustering[i];
                clusters[c][clustersCurIdx[c]] = items[i];
                ++clustersCurIdx[c];
            }

            return new Result<T>(clusters, totalDistance);
        }

        private static double[][] ConvertToVectors<T>(T[] items)
        {
            var type = typeof(T);
            var data = new List<double[]>();

            // If the type is an array type
            if (type.IsArray && type.IsAssignableFrom(typeof(double[])))
            {
                foreach (var item in items)
                {
                    var val = item as double[];
                    data.Add(val);
                }
                return data.ToArray();
            }

            var getters = new List<MethodInfo>();

            // Iterate over the type and extract all the properties that have the KMeansValueAttribute set and use them as attributes
            var attribType = typeof(KMeansValueAttribute);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attribs = property.GetCustomAttributes(attribType, false).OfType<KMeansValueAttribute>().ToArray();
                if (attribs.Length <= 0)
                    continue;

                var getter = property.GetGetMethod();
                if (getter == null)
                    throw new InvalidOperationException("No public getter for property '" + property.Name + "'. All properties marked with the KMeansValueAttribute must have a public getter");

                if (!property.PropertyType.IsAssignableFrom(typeof(double)) &&
                    !property.PropertyType.IsAssignableFrom(typeof(int)) &&
                    !property.PropertyType.IsAssignableFrom(typeof(float)) &&
                    !property.PropertyType.IsAssignableFrom(typeof(long)) &&
                    !property.PropertyType.IsAssignableFrom(typeof(decimal)) &&
                    !property.PropertyType.IsAssignableFrom(typeof(short)))
                    throw new InvalidOperationException("Property type '" + property.PropertyType.Name + "' for property '" + property.Name + "' cannot be assigned to System.Double. ");

                getters.Add(getter);
            }

            foreach (var item in items)
            {
                var values = new List<double>(getters.Count);
                foreach (var getter in getters)
                    values.Add(Convert.ToDouble(getter.Invoke(item, null)));
                data.Add(values.ToArray());
            }

            return data.ToArray();
        }

        private static int[] InitializeClustering(int numData, int clusterCount)
        {
            var rnd = new Random();
            var clustering = new int[numData];
            for (var i = 0; i < numData; ++i)
            {
                clustering[i] = rnd.Next(0, clusterCount);
            }

            return clustering;
        }

        private static double[][] CreateMatrix(int rows, int columns)
        {
            var matrix = new double[rows][];
            for (var i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new double[columns];
            }

            return matrix;
        }

        private static double CalculateClusteringInformation(
            double[][] data, 
            int[] clustering, 
            ref int[] centroidIdx,
            int clusterCount, 
            ref int[] clusterItemCount, 
            KMeansCalculateDistanceDelegate calculateDistanceFunction)
        {
            var means = CreateMatrix(clusterCount, data[0].Length);

            // Calculate the means for each cluster
            // Do this in two phases, first sum them all up and then divide by the count in each cluster
            for (var i = 0; i < data.Length; i++)
            {
                // Sum up the means
                var row = data[i];
                var clusterIdx = clustering[i]; // What cluster is data i assigned to
                ++clusterItemCount[clusterIdx]; // Increment the count of the cluster that row i is assigned to
                for (var j = 0; j < row.Length; j++)
                {
                    means[clusterIdx][j] += row[j];
                }
            }

            // Now divide to get the average
            for (var k = 0; k < means.Length; k++)
            {
                for (var a = 0; a < means[k].Length; a++)
                {
                    var itemCount = clusterItemCount[k];
                    means[k][a] /= itemCount > 0 ? itemCount : 1;
                }
            }

            double totalDistance = 0;
            var minDistances = new double[clusterCount].Select(x => double.MaxValue).ToArray();
            for (var i = 0; i < data.Length; i++)
            {
                var clusterIdx = clustering[i]; // What cluster is data i assigned to
                var distance = calculateDistanceFunction(data[i], means[clusterIdx]);
                totalDistance += distance;
                if (distance < minDistances[clusterIdx])
                {
                    minDistances[clusterIdx] = distance;
                    centroidIdx[clusterIdx] = i;
                }
            }

            return totalDistance;
        }

        private static bool AssignClustering(double[][] data, int[] clustering, int[] centroidIdx, int clusterCount, KMeansCalculateDistanceDelegate calculateDistanceFunction)
        {
            var changed = false;

            for (var i = 0; i < data.Length; i++)
            {
                var minDistance = double.MaxValue;
                var minClusterIndex = -1;

                for (var k = 0; k < clusterCount; k++)
                {
                    var distance = calculateDistanceFunction(data[i], data[centroidIdx[k]]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minClusterIndex = k;
                    }
                }

                // Re-arrange the clustering for datapoint if needed
                if (minClusterIndex != -1 && clustering[i] != minClusterIndex)
                {
                    changed = true;
                    clustering[i] = minClusterIndex;
                }
            }

            return changed;
        }

        public class Result<T>
        {
            public T[][] Clusters { get; }

            public double TotalDistance { get; }

            public Result(T[][] clusters, double totalDistance)
            {
                Clusters = clusters;
                TotalDistance = totalDistance;
            }
        }
    }
}
