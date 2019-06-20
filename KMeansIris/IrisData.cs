using System.Diagnostics;

namespace KMeansIris
{
    [DebuggerDisplay("{Species}")]
    public class IrisData
    {
        public string Id { get; set; }

        [KMeansValue] public float SepalLength { get; set; }

        [KMeansValue] public float SepalWidth { get; set; }

        [KMeansValue] public float PetalLength { get; set; }

        [KMeansValue] public float PetalWidth { get; set; }

        public string Species { get; set; }
    }
}