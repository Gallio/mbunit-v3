using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Mathematics
{
    /// <summary>
    /// Helper methods to calculate the Gamma function (http://en.wikipedia.org/wiki/Gamma_function).
    /// </summary>
    public static class Gamma
    {
        private const int MaxIterations = 100; // Maximum allowed number of iterations.
        private const double Accuracy = 3E-7; // Relative accuracy
        private const double Epsilon = 1E-30;

        /// <summary>
        /// Returns the incomplete Gamma function Q(a,x).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double IncompleteGamma(double a, double x)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException("x");
            if (a <= 0)
                throw new ArgumentOutOfRangeException("a");

            if (x < a + 1)
            {
                // Use the series representation and take its complement.
                return 1 - IncompleteGammaSeries(a, x);
            }

            // Use the continued fraction representation.
            return IncompleteGammaContinuedFraction(a, x);
        }

        // Returns the incomplete Gamma function evaluated by its series representation.
        private static double IncompleteGammaSeries(double a, double x)
        {
            if (x <= 0)
                throw new ArgumentOutOfRangeException("a");

            double ap = a;
            double delta = 1 / a;
            double sum = delta;

            for (int n = 1; n < MaxIterations; n++)
            {
                ap++;
                delta *= x / ap;
                sum += delta;

                if (Math.Abs(delta) < Math.Abs(sum) * Accuracy)
                    return sum * Math.Exp(-x + a * Math.Log(x) - GammaLogarithm(a));
            }

            throw new ArgumentOutOfRangeException("a", "Value too large.");
        }

        // Returns the incomplete Gamma function evaluated by its continued fraction representation.
        private static double IncompleteGammaContinuedFraction(double a, double x)
        {
            double b = x + 1 - a;
            double c = 1 / Epsilon;
            double d = 1 / b;
            double h = d;

            for (int i = 1; i < MaxIterations; i++)
            {
                double an = -i * (i - a);
                b += 2;
                d = 1 / Math.Max(Epsilon, an * d + b);
                c = Math.Max(Epsilon, b + an / c);
                double delta = d * c;
                h *= delta;

                if (Math.Abs(delta - 1) < Accuracy)
                    return Math.Exp(-x + a * Math.Log(x) - GammaLogarithm(a)) * h;
            }

            throw new ArgumentOutOfRangeException("a", "Value too large.");
        }

        // Returns the natural logarithm of the specified positive Gamma value.
        private static double GammaLogarithm(double a)
        {
            var coef = new[]
            {
               76.18009172947146,
               -86.50532032941677,
               24.01409824083091,
               -1.231739572450155,
               0.1208650973866179E-2,
               -0.5395239384953E-5
            };

            double x = a;
            double y = a;
            double t = x + 5.5;
            t -= (x + 0.5) * Math.Log(t);
            double s = 1.000000000190015;

            for (int j = 0; j < coef.Length; j++)
            {
                s += coef[j] / (++y);
            }

            return -t + Math.Log(2.5066282746310005 * s / x);
        }
    }
}
