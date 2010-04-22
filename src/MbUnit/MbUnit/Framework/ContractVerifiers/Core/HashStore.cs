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
        private readonly IDictionary<int, double> map = new Dictionary<int, double>();
        private readonly double count;

        /// <summary>
        /// Gets the total number of hash codes stored in the map.
        /// </summary>
        public double Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Constructs a map from the specified hash codes.
        /// </summary>
        /// <param name="hashes">An enumeration of hash codes.</param>
        public HashStore(IEnumerable<int> hashes)
        {
            int n = 0;

            foreach(int hash in hashes)
            {
                Add(hash);
                n++;
            }

            count = n;
        }

        private void Add(int hash)
        {
            double occurences;
            bool exists = map.TryGetValue(hash, out occurences);
            map[hash] = 1.0 + (exists ? occurences : 0.0);
        }

        /// <summary>
        /// Gets the number of occurences for the specified hash code.
        /// </summary>
        /// <param name="hash">The searched hash code.</param>
        /// <returns>The number of occurences found.</returns>
        public double this[int hash]
        {
            get
            {
                double occurences;
                return map.TryGetValue(hash, out occurences) ? occurences : 0;
            }
        }

        /// <summary>
        /// Returns the probability of collision.
        /// </summary>
        /// <returns>The probability of collision between 0 and 1.</returns>
        public double GetCollisionProbability()
        {
            double result = 0.0;

            foreach (var pair in map)
            {
                if (pair.Value > 1)
                {
                    double occurences = pair.Value;
                    result += (occurences / count) * ((occurences - 1.0) / (count - 1.0));
                }
            }

            return result;
        }
        
        /// <summary>
        /// Returns the Chi-Square test result.
        /// </summary>
        /// <returns>The resulting probability level.</returns>
        public ChiSquareTest GetChiSquareGoodnessToFit()
        {
            int k = GetPrime(map.Count);
            var actual = new double[k];
            Array.Clear(actual, 0, k);
            int i = 0;

            foreach (var value in map.Values)
            {
                actual[i % k] += value;
                i++;
            }

            var expected = CollectionUtils.ConstantArray(count / k, k);
            return new ChiSquareTest(expected, actual, 1);
        }

        private static int GetPrime(int n)
        {
            if (n > 400)
                return 113;

            if (n > 80)
                return 19;

            return n;
        }
    }
}
