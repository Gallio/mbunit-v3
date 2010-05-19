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
