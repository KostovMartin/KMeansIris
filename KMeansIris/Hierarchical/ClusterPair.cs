using System;
using System.Collections.Generic;

namespace KMeansIris.Hierarchical
{
    public class ClusterPair
    {
        public ClusterPair()
        {
        }

        public ClusterPair(Cluster cluster1, Cluster cluster2)
        {
            this.Cluster1 = cluster1 ?? throw new ArgumentNullException("cluster1");
            this.Cluster2 = cluster2 ?? throw new ArgumentNullException("cluster2");
        }

        public Cluster Cluster1 { get; set; }

        public Cluster Cluster2 { get; set; }

        public class EqualityComparer : IEqualityComparer<ClusterPair>
        {
            public bool Equals(ClusterPair x, ClusterPair y)
            {
                return x.Cluster1.Id == y.Cluster1.Id && x.Cluster2.Id == y.Cluster2.Id;
            }

            public int GetHashCode(ClusterPair x)
            {
                return x.Cluster1.Id ^ x.Cluster2.Id;
            }
        }
    }
}
