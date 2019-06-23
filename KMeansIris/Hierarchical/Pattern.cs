using System.Collections.Generic;
using System.Linq;

namespace KMeansIris.Hierarchical
{
    public class Pattern
    {
        private readonly List<double> _attributeCollection;

        public Pattern()
        {
            _attributeCollection = new List<double>();
        }

        public int Id { get; set; }

        public void AddAttributes(double[] attributes)
        {
            _attributeCollection.AddRange(attributes);
        }

        public double[] GetAttributes()
        {
            return _attributeCollection.ToArray<double>();
        }
    }
}
