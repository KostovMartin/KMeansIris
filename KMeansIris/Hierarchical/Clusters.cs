using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KMeansIris.Hierarchical
{
    public class Clusters : IEnumerable<Cluster>
    {
        private HashSet<Cluster> _clusters;

        public Clusters()
        {
            _clusters = new HashSet<Cluster>();
        }

        public void AddCluster(Cluster cluster)
        {
            _clusters.Add(cluster);
        }

        public void RemoveCluster(Cluster cluster)
        {
            _clusters.Remove(cluster);
        }

        public Cluster GetCluster(int index)
        {
            return _clusters.ElementAt(index);
        }

        public int Count()
        {
            return _clusters.Count;
        }

        public void BuildSingletonCluster(HashSet<Pattern> patternMatrix)
        {
            int clusterId = 0;
            Cluster cluster;

            foreach (Pattern item in patternMatrix)
            {
                cluster = new Cluster();
                cluster.Id = clusterId;
                cluster.AddPattern(item);
                cluster.TotalQuantityOfPatterns = 1;
                _clusters.Add(cluster);
                clusterId++;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _clusters.GetEnumerator();
        }

        IEnumerator<Cluster> IEnumerable<Cluster>.GetEnumerator()
        {
            return _clusters.GetEnumerator();
        }
    }
}
