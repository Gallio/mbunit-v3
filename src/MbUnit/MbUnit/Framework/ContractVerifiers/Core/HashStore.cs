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
    public class HashStore
    {
        private const int minimumHashCount = 2;
        private readonly HashSet<int> one = new HashSet<int>();
        private readonly HashSet<int> two = new HashSet<int>();
        private readonly IDictionary<int, int> more = new Dictionary<int, int>();
        private readonly HashStoreResult result;

        /// <summary>
        /// Gets the results of the analysis.
        /// </summary>
        public HashStoreResult Result
        {
            get
            {
                return result;
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
                Add(hash);
                count++;
            }

            if (count < minimumHashCount)
                throw new NotEnoughHashesException(minimumHashCount, count);

            result = CalculateResults(count);
        }

        private void Add(int hash)
        {
            if (one.Contains(hash))
            {
                one.Remove(hash);
                two.Add(hash);
            }
            else if (two.Contains(hash))
            {
                two.Remove(hash);
                more.Add(hash, 3);
            }
            else
            {
                int n;

                if (more.TryGetValue(hash, out n))
                {
                    more[hash] = 1 + n;
                }
                else
                {
                    one.Add(hash);
                }
            }
        }

        internal int this[int hash]
        {
            get
            {
                if (one.Contains(hash))
                    return 1;

                if (two.Contains(hash))
                    return 2;
                
                int n;
                return more.TryGetValue(hash, out n) ? n : 0;
            }
        }

        private HashStoreResult CalculateResults(int count)
        {
            int bucketSize = GetBucketSize();
            var bucket = new double[bucketSize];
            double collisionProbability = 0;
            int index = 0;

            for (int i = 0; i < one.Count; i++)
            {
                bucket[index++ % bucketSize] += 1;
            }

            for (int i = 0; i < two.Count; i++)
            {
                bucket[index++ % bucketSize] += 2;
                collisionProbability += 2.0 / (count * (count - 1));
            }

            foreach (var pair in more)
            {
                bucket[index++ % bucketSize] += pair.Value;
                collisionProbability += (double)pair.Value / count * (pair.Value - 1) / (count - 1);
            }

            var chiSquareTest = new ChiSquareTest((double)count / bucketSize, bucket, 1);
            double uniformDistributionDeviationProbability = 1.0 - chiSquareTest.TwoTailedPValue;
            return new HashStoreResult(count, collisionProbability, uniformDistributionDeviationProbability);
        }

        private int GetBucketSize()
        {
            const int threshold = 10;
            var primes = new[] { 14813, 3613, 223, 17 };
            int count = one.Count + two.Count + more.Count;

            foreach (int prime in primes)
            {
                if (count >= prime * threshold)
                    return prime;
            }

            return count;
        }
    }
}
