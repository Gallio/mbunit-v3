using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Mathematics;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// A map that stores the occurences of hash code values and computes various statistical data.
    /// </summary>
    internal class HashStore
    {
        private readonly IDictionary<int, int> map = new Dictionary<int, int>();
        private readonly int total;
        private double collisionProbability;
        private double uniformDistributionDeviationProbability;

        /// <summary>
        /// Gets the total number of hash codes stored in the map.
        /// </summary>
        public int StatisticalPopulation
        {
            get
            {
                return total;
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
        /// Gets the probability of deviation from uniform distribution.
        /// </summary>
        public double UniformDistributionDeviationProbability
        {
            get
            {
                return uniformDistributionDeviationProbability;
            }
        }

        /// <summary>
        /// Constructs a map from the specified hash codes.
        /// </summary>
        /// <param name="hashes">An enumeration of hash codes.</param>
        public HashStore(IEnumerable<int> hashes)
        {
            int count = 0;

            foreach (int hash in hashes)
            {
                int n;
                map[hash] = 1 + (map.TryGetValue(hash, out n) ? n : 0);
                count++;
            }

            total = count;
            CalculateStatistics();
        }

        internal int this[int hash]
        {
            get
            {
                int n;
                return map.TryGetValue(hash, out n) ? n : 0;
            }
        }

        private void CalculateStatistics()
        {
            int bucketSize = GetBucketSize();
            var bucket = new double[bucketSize];
            collisionProbability = 0;

            foreach(var pair in map)
            {
                bucket[pair.Key % bucketSize] += pair.Value;

                if (pair.Value > 1)
                {
                    collisionProbability += (double)pair.Value / total * (pair.Value - 1) / (total - 1);
                }
            }

            Gallio.Framework.TestLog.WriteLine("Total = {0}", total);
            Gallio.Framework.TestLog.WriteLine("bucketSize = {0}", bucketSize);
            Gallio.Framework.TestLog.WriteLine("bucket = {0} {1}", bucket[0], bucket[1]);
            var chiSquareTest = new ChiSquareTest((double)total / bucketSize, bucket, 1);
            uniformDistributionDeviationProbability = 1.0 - chiSquareTest.TwoTailedPValue;
        }

        private int GetBucketSize()
        {
            const int threshold = 3;
            var primes = new[] {103451, 14813, 3613, 223};

            foreach (int prime in primes)
            {
                if (map.Count >= prime * threshold)
                    return prime;
            }

            return map.Count;
        }
    }
}
