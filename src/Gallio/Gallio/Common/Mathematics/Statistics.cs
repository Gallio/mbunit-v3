// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Mathematics
{
/*
    Copyright (C) 1984 Stephen L. Moshier (original C version - Cephes Math Library)
    Copyright (C) 1996 Leigh Brookshaw	(Java version)
    Copyright (C) 2005 Miroslav Stampar	(C# version [->this<-])

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA. 
*/

    /// <summary>
    /// Special statistical functions.
    /// </summary>
    public static class Statistics
    {
        private const double MachEp = 1.11022302462515654042E-16;
        private const double MaxLog = 7.09782712893383996732E2;
        private const double LogPi = 1.14472988584940017414;

        /// <summary>
        /// Returns the area under the right hand tail (from x to infinity) 
        /// of the Chi-Square probability density function with k degrees of freedom.
        /// </summary>
        /// <param name="degreesOfFreedom">Degrees of freedom</param>
        /// <param name="chiSquareValue">The Chi-Square value.</param>
        /// <returns></returns>
        public static double ChiSquareProbability(double degreesOfFreedom, double chiSquareValue)
        {
            if (chiSquareValue < 0 || degreesOfFreedom < 1)
                return 0;

            return ComplementedIncompleteGamma(degreesOfFreedom / 2, chiSquareValue / 2);
        }

        /// <summary>
        /// Returns the complemented incomplete gamma function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ComplementedIncompleteGamma(double a, double x)
        {
            if (x <= 0 || a <= 0)
                return 1.0;

            if (x < 1.0 || x < a)
                return 1.0 - IncompleteGamma(a, x);

            const double big = 4.503599627370496e15;
            const double biginv = 2.22044604925031308085e-16;
            double ax = a * Math.Log(x) - x - GammaLogarithm(a);

            if (ax < -MaxLog)
                return 0.0;

            ax = Math.Exp(ax);
            double y = 1.0 - a;
            double z = x + y + 1.0;
            double c = 0.0;
            double pkm2 = 1.0;
            double qkm2 = x;
            double pkm1 = x + 1.0;
            double qkm1 = z * x;
            double ans = pkm1 / qkm1;
            double t;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                double yc = y * c;
                double pk = pkm1 * z - pkm2 * yc;
                double qk = qkm1 * z - qkm2 * yc;

                if (qk != 0)
                {
                    double r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (Math.Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }

            } while (t > MachEp);

            return ans * ax;
        }

    	/// <summary>
    	/// Returns the incomplete gamma function.
    	/// </summary>
    	/// <param name="a"></param>
    	/// <param name="x"></param>
    	/// <returns></returns>
    	public static double IncompleteGamma(double a, double x)
    	{
    	    if (x <= 0 || a <= 0) 
                return 0.0;

            if (x > 1.0 && x > a)
                return 1.0 - ComplementedIncompleteGamma(a, x);

            double ax = a * Math.Log(x) - x - GammaLogarithm(a);
    		
            if (ax < -MaxLog) 
                return (0.0);
    		
            ax = Math.Exp(ax);
    		double r = a;
    		double c = 1.0;
    		double ans = 1.0;

    		do
    		{
    			r += 1.0;
    			c *= x/r;
    			ans += c;

    		} while (c / ans > MachEp);

    		return (ans * ax / a);
    	}

        /// <summary>
        /// Returns the natural logarithm of gamma function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GammaLogarithm(double x)
        {
            double p, q, z;

            double[] A =
            {
                8.11614167470508450300E-4,
                -5.95061904284301438324E-4,
                7.93650340457716943945E-4,
                -2.77777777730099687205E-3,
                8.33333333333331927722E-2
            };

            double[] B =
            {
                -1.37825152569120859100E3,
                -3.88016315134637840924E4,
                -3.31612992738871184744E5,
                -1.16237097492762307383E6,
                -1.72173700820839662146E6,
                -8.53555664245765465627E5
            };

            double[] C =
            {
                -3.51815701436523470549E2,
                -1.70642106651881159223E4,
                -2.20528590553854454839E5,
                -1.13933444367982507207E6,
                -2.53252307177582951285E6,
                -2.01889141433532773231E6
            };

            if (x < -34.0)
            {
                q = -x;
                double w = GammaLogarithm(q);
                p = Math.Floor(q);

                if (p == q)
                    throw new ArithmeticException("GammaLogarithm: Overflow");

                z = q - p;

                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }

                z = q * Math.Sin(Math.PI * z);

                if (z == 0.0)
                    throw new ArithmeticException("GammaLogarithm: Overflow");

                z = LogPi - Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;

                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }

                while (x < 2.0)
                {
                    if (x == 0.0)
                        throw new ArithmeticException("GammaLogarithm: Overflow");

                    z /= x;
                    x += 1.0;
                }

                if (z < 0.0)
                    z = -z;

                if (x == 2.0)
                    return Math.Log(z);

                x -= 2.0;
                p = x * EvaluatePolynomial(x, B) / EvaluatePolynomialWithN1(x, C);
                return (Math.Log(z) + p);
            }

            if (x > 2.556348e305)
                throw new ArithmeticException("GammaLogarithm: Overflow");

            q = (x - 0.5) * Math.Log(x) - x + 0.91893853320467274178;

            if (x > 1.0e8)
                return (q);

            p = 1.0 / (x * x);

            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4 * p - 2.7777777777777777777778e-3) * p + 0.0833333333333333333333) / x;
            }
            else
            {
                q += EvaluatePolynomial(p, A) / x;
            }

            return q;
        }

        /// <summary>
        /// Evaluates polynomial of degree N
        /// </summary>
        /// <param name="x">The X-value used to evaluate the polynomial.</param>
        /// <param name="coef">An array of N coefficients.</param>
        /// <returns>The calculated value.</returns>		
        private static double EvaluatePolynomial(double x, double[] coef)
        {
            double result = coef[0];

            for (int i = 1; i < coef.Length; i++)
            {
                result = result * x + coef[i];
            }

            return result;
        }

        /// <summary>
        /// Evaluates polynomial of degree N with assumption that coef[N] = 1.0
        /// </summary>
        /// <param name="x">The X-value used to evaluate the polynomial.</param>
        /// <param name="coef">An array of N coefficients.</param>
        /// <returns>The calculated value.</returns>		
        private static double EvaluatePolynomialWithN1(double x, double[] coef)
        {
            double result = x + coef[0];

            for (int i = 1; i < coef.Length - 1; i++)
            {
                result = result * x + coef[i];
            }

            return result;
        }
    }
}
