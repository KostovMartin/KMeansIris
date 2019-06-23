using System.Collections.Generic;
using System.Threading.Tasks;
using KMeansIris;

namespace KMeansIris.Hierarchical
{
    public class AgnesClustering
    {
        private Pattern _pattern;
        private HashSet<Pattern> _patternMatrix; 
        private int _patternIndex = 0;
        private Clusters _clusters; // data structure for clustering
        private DissimilarityMatrix _dissimilarityMatrix;
        private readonly DistanceDelegate _distanceFunction;

        public AgnesClustering(DistanceDelegate distanceFunction, HashSet<double[]> dataSet)
        {
            _clusters = new Clusters();
            _patternMatrix = new HashSet<Pattern>();

            foreach (var item in dataSet)
            {
                _pattern = new Pattern();
                _pattern.Id = _patternIndex;
                _pattern.AddAttributes(item);
                _patternMatrix.Add(_pattern);
                _patternIndex++;
            }

            this._distanceFunction = distanceFunction;
        }

        public Clusters Cluster(ClusterDistanceStrategy strategy, int k)
        {
            // build a clustering only with singleton clusters
            this._BuildSingletonCluster();

            //build the dissimilarity matrix
            this._BuildDissimilarityMatrixParallel();

            // build the hierarchical clustering 
            this.BuildHierarchicalClustering(_clusters.Count(), strategy, k);

            return _clusters;
        }

        private void _BuildDissimilarityMatrixParallel()
        {
            _dissimilarityMatrix = new DissimilarityMatrix();

            Parallel.ForEach(_ClusterPairCollection(), clusterPair =>
            {
                double distanceBetweenTwoClusters = 0;
                if (clusterPair.Cluster1.QuantityOfPatterns() == 1 && clusterPair.Cluster2.QuantityOfPatterns() == 1)
                {
                    distanceBetweenTwoClusters = _distanceFunction(
                        clusterPair.Cluster1.GetPattern(0).GetAttributes(), 
                        clusterPair.Cluster2.GetPattern(0).GetAttributes());
                }
                               
                _dissimilarityMatrix.AddClusterPairAndDistance(clusterPair, distanceBetweenTwoClusters);
            });
        }

        private IEnumerable<ClusterPair> _ClusterPairCollection()
        {
            ClusterPair clusterPair;

            for (int i = 0; i < _clusters.Count(); i++)
            {
                for (int j = i + 1; j < _clusters.Count(); j++)
                {
                    clusterPair = new ClusterPair();
                    clusterPair.Cluster1 = _clusters.GetCluster(i);
                    clusterPair.Cluster2 = _clusters.GetCluster(j);

                    yield return clusterPair;
                }
            }

        }

        private void _BuildSingletonCluster()
        {
            _clusters.BuildSingletonCluster(_patternMatrix);
        }

        private void _UpdateDissimilarityMatrix(Cluster newCluster, ClusterDistanceStrategy strategie)
        {
            double distanceBetweenClusters;
            for (int i = 0; i < _clusters.Count(); i++)
            {
                // compute the distance between old clusters to the new cluster
                distanceBetweenClusters = ComputeDistance(_clusters.GetCluster(i), newCluster, _dissimilarityMatrix, strategie);
                // insert the new cluster's distance
                _dissimilarityMatrix.AddClusterPairAndDistance(new ClusterPair(newCluster, _clusters.GetCluster(i)), distanceBetweenClusters);
                //remove all old distance values of the old clusters (subclusters of the newcluster)
                _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(0), _clusters.GetCluster(i)));
                _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(1), _clusters.GetCluster(i)));
            }

            // finally, remove the distance of the old cluster pair
            _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(0), newCluster.GetSubCluster(1)));
        }

        private ClusterPair _GetClosestClusterPairInDissimilarityMatrix()
        {
            return _dissimilarityMatrix.GetClosestClusterPair();
        }

        private void BuildHierarchicalClustering(int indexNewCluster, ClusterDistanceStrategy strategy, int k)
        {

            ClusterPair closestClusterPair = this._GetClosestClusterPairInDissimilarityMatrix();

            // create a new cluster by merge the closest cluster pair
            Cluster newCluster = new Cluster();
            newCluster.AddSubCluster(closestClusterPair.Cluster1);
            newCluster.AddSubCluster(closestClusterPair.Cluster2);
            newCluster.Id = indexNewCluster;
            newCluster.UpdateTotalQuantityOfPatterns(); //update the total quantity of patterns of the new cluster (this quantity is used by UPGMA clustering strategy)
            _clusters.RemoveCluster(closestClusterPair.Cluster1);
            _clusters.RemoveCluster(closestClusterPair.Cluster2);
            _UpdateDissimilarityMatrix(newCluster, strategy);
            //add the new cluster to clustering
            _clusters.AddCluster(newCluster);

            // recursive call of this method while there is more than 1 cluster (k>2) in the clustering
            if (_clusters.Count() > k)
                this.BuildHierarchicalClustering(indexNewCluster + 1, strategy, k);
        }

        private double ComputeDistance(Cluster cluster1, Cluster cluster2, DissimilarityMatrix dissimilarityMatrix, ClusterDistanceStrategy strategy)
        {
            var distance1 = dissimilarityMatrix.ReturnClusterPairDistance(new ClusterPair(cluster1, cluster2.GetSubCluster(0)));
            var distance2 = dissimilarityMatrix.ReturnClusterPairDistance(new ClusterPair(cluster1, cluster2.GetSubCluster(1)));

            switch (strategy)
            {
                case ClusterDistanceStrategy.SingleLinkage: return distance1 < distance2 ? distance1 : distance2;
                case ClusterDistanceStrategy.CompleteLinkage: return distance1 > distance2 ? distance1 : distance2;
                case ClusterDistanceStrategy.AverageWeightedPairGroupMethodArithmeticMean: return (distance1 + distance2) / 2;
                case ClusterDistanceStrategy.AverageUnweightedPairGroupMethodArithmeticMean:
                    return ((cluster2.GetSubCluster(0).TotalQuantityOfPatterns * distance1) / cluster2.TotalQuantityOfPatterns) +
                           ((cluster2.GetSubCluster(1).TotalQuantityOfPatterns * distance2) / cluster2.TotalQuantityOfPatterns);
                default: return 0;
            }
        }
    }
}
