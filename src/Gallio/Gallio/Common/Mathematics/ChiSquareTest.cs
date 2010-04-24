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
using System.Threading;

namespace Gallio.Common.Mathematics
{
    /// <summary>
    /// Implementation of a Chi-square test (http://en.wikipedia.org/wiki/Chi_square_test)
    /// that calculates the "difference" between an expected and an actual statistical distribution.
    /// </summary>
    public class ChiSquareTest
    {
        private readonly int degreesOfFreedom;
        private readonly double chiSquareValue;
        private readonly double twoTailedPValue;

        /// <summary>
        /// Gets the resulting number of degrees of freedom.
        /// </summary>
        public int DegreesOfFreedom
        {
            get
            {
                return degreesOfFreedom;
            }
        }

        /// <summary>
        /// Gets the resulting Chi-square value.
        /// </summary>
        public double ChiSquareValue
        {
            get
            {
                return chiSquareValue;
            }
        }

        /// <summary>
        /// Gets the resulting significance probability.
        /// </summary>
        public double TwoTailedPValue
        {
            get
            {
                return twoTailedPValue;
            }
        }

        /// <summary>
        /// Runs a Chi-square test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Given the observed events (<paramref name="actual"/>) and the expected events (<paramref name="expected"/>), 
        /// calculates the number of degrees of freedom, the chi-square value, and the significance probability.
        /// </para>
        /// <para>
        /// A small value of the significance probability indicates a significant difference between the distributions.
        /// </para>
        /// </remarks>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="numberOfConstraints"></param>
        public ChiSquareTest(double expected, ICollection<double> actual, int numberOfConstraints)
        {
            if (expected <= 0)
                throw new ArgumentOutOfRangeException("The expected value is negative.", "expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            degreesOfFreedom = actual.Count - numberOfConstraints;

            foreach (double value in actual)
            {
                var delta = value - expected;
                chiSquareValue += (delta * delta) / expected;
            }

            twoTailedPValue = Gamma.IncompleteGamma(degreesOfFreedom / 2d, chiSquareValue / 2d);
        }
    }
}
