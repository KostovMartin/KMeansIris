using System;

namespace KMeansIris
{
    public class DistanceFunctions
    {
        public static double EuclideanDistance(double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                sum += u * u;
            }

            return Math.Sqrt(sum);
        }

        public static double ManhattanDistance(double[] x, double[] y)
        {
            return MinkowskiDistance(x, y, 1);
        }

        public static double CosineDistance(double[] x, double[] y)
        {
            double sum = 0;
            double p = 0;
            double q = 0;

            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * y[i];
                p += x[i] * x[i];
                q += y[i] * y[i];
            }

            double den = Math.Sqrt(p) * Math.Sqrt(q);
            return sum == 0 ? 1.0 : 1.0 - (sum / den);
        }

        public static double ChebyshevDistance(double[] x, double[] y)
        {
            double max = Math.Abs(x[0] - y[0]);

            for (int i = 1; i < x.Length; i++)
            {
                double abs = Math.Abs(x[i] - y[i]);
                if (abs > max)
                {
                    max = abs;
                }
            }

            return max;
        }

        private static double MinkowskiDistance(double[] x, double[] y, int p)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += Math.Pow(Math.Abs(x[i] - y[i]), p);
            }

            return Math.Pow(sum, 1 / p);
        }
    }
}
