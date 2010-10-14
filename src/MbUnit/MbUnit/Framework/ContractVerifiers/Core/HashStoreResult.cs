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

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// Simple container for hash code stochastic analysis results.
    /// </summary>
    public sealed class HashStoreResult
    {
        private readonly int statisticalPopulation;
        private readonly double collisionProbability;
        private readonly double uniformDistributionDeviationProbability;

        /// <summary>
        /// Gets the total number of hash codes used to perform the analysis.
        /// </summary>
        public int StatisticalPopulation
        {
            get
            {
                return statisticalPopulation;
            }
        }

        /// <summary>
        /// Gets the probability that two values randomly chosen have the same hash code.
        /// </summary>
        public double CollisionProbability
        {
            get
            {
                return collisionProbability;
            }
        }

        /// <summary>
        /// Gets the probability of deviation from a uniform distribution.
        /// </summary>
        public double UniformDistributionDeviationProbability
        {
            get
            {
                return uniformDistributionDeviationProbability;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statisticalPopulation">The total number of hash codes used to perform the analysis.</param>
        /// <param name="collisionProbability">The probability that two values randomly chosen have the same hash code.</param>
        /// <param name="uniformDistributionDeviationProbability">The probability of deviation from a uniform distribution.</param>
        public HashStoreResult(int statisticalPopulation, double collisionProbability, double uniformDistributionDeviationProbability)
        {
            this.statisticalPopulation = statisticalPopulation;
            this.collisionProbability = collisionProbability;
            this.uniformDistributionDeviationProbability = uniformDistributionDeviationProbability;
        }
    }
}
