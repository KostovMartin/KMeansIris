using System.Collections.Concurrent;

namespace KMeansIris.Hierarchical
{
    public class DissimilarityMatrix
    {
        private ConcurrentDictionary<ClusterPair, double> _distanceMatrix;

        public DissimilarityMatrix()
        {
            _distanceMatrix = new ConcurrentDictionary<ClusterPair, double>(new ClusterPair.EqualityComparer());
        }

        public void AddClusterPairAndDistance(ClusterPair clusterPair, double distance)
        {
            _distanceMatrix.TryAdd(clusterPair, distance);
        }

        public void RemoveClusterPair(ClusterPair clusterPair)
        {
            if (_distanceMatrix.ContainsKey(clusterPair))
            {
                _distanceMatrix.TryRemove(clusterPair, out _);
            }
            else
            {
                _distanceMatrix.TryRemove(new ClusterPair(clusterPair.Cluster2, clusterPair.Cluster1), out _);
            }
        }

        public ClusterPair GetClosestClusterPair()
        {
            double minDistance = double.MaxValue;
            ClusterPair closestClusterPair = new ClusterPair();

            foreach (var item in _distanceMatrix)
            {
                if(item.Value < minDistance)
                {
                    minDistance = item.Value;
                    closestClusterPair = item.Key;
                }
            }

            return closestClusterPair;
        }

        public double ReturnClusterPairDistance(ClusterPair clusterPair)
        {
            if (_distanceMatrix.ContainsKey(clusterPair))
                return _distanceMatrix[clusterPair];
            else
                return _distanceMatrix[new ClusterPair(clusterPair.Cluster2, clusterPair.Cluster1)];
        }
    }
}
