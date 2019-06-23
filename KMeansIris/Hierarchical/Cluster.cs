using System.Collections.Generic;
using System.Linq;

namespace KMeansIris.Hierarchical
{
    public class Cluster
    {
        private HashSet<Pattern> _cluster;
        private HashSet<Cluster> _subClusters;
        private List<Pattern> _patternList;

        public int Id { get; set; }

        public int TotalQuantityOfPatterns { get; set; } = 0;

        public Cluster()
        {
            _cluster = new HashSet<Pattern>();
            _subClusters = new HashSet<Cluster>();
        }

        public void AddPattern(Pattern pattern)
        {
            _cluster.Add(pattern);
        }

        public int QuantityOfPatterns()
        {
            return _cluster.Count;
        }

        public Pattern[] GetPatterns()
        {
            return _cluster.ToArray();
        }

        public Pattern GetPattern(int index)
        {
            return _cluster.ElementAt(index);
        }

        public void AddSubCluster(Cluster subCluster)
        {
            _subClusters.Add(subCluster);
        }

        public Cluster[] GetSubClusters()
        {
            return _subClusters.ToArray();
        }

        public int QuantityOfSubClusters()
        {
            return _subClusters.Count;
        }

        public Cluster GetSubCluster(int index)
        {
            return _subClusters.ElementAt(index);
        }

        public int UpdateTotalQuantityOfPatterns()
        {
            if (this.GetSubClusters().Count() > 0)
            {
                TotalQuantityOfPatterns = 0;
                foreach (Cluster subcluster in this.GetSubClusters())
                {
                    TotalQuantityOfPatterns = TotalQuantityOfPatterns + subcluster.UpdateTotalQuantityOfPatterns();
                }
            }

            return TotalQuantityOfPatterns;
        }

        public List<Pattern> GetAllPatterns()
        {
            _patternList = new List<Pattern>();
            if (this.QuantityOfSubClusters() == 0)
            {
                foreach (Pattern pattern in this.GetPatterns())
                    _patternList.Add(pattern);
            }
            else
            {
                foreach (Cluster subCluster in this.GetSubClusters())
                    _GetSubClusterPattern(subCluster);
            }
   
            return _patternList;
        }

        private void _GetSubClusterPattern(Cluster subCluster)
        {
            if (subCluster.QuantityOfSubClusters() == 0)
            {
                foreach (Pattern pattern in subCluster.GetPatterns())
                    _patternList.Add(pattern);
            }
            else
            {
                foreach (Cluster _subCluster in subCluster.GetSubClusters())
                    _GetSubClusterPattern(_subCluster);
            }
        }
    }
}
