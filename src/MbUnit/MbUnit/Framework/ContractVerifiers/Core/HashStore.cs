using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// A map that stores the occurences of hash code values and computes various statistical data.
    /// </summary>
    internal class HashStore
    {
        private readonly IDictionary<int, int> map = new Dictionary<int, int>();
        private readonly int count;

        /// <summary>
        /// Gets the total number of hash codes stored in the map.
        /// </summary>
        public int Count
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
            int occurences;
            bool exists = map.TryGetValue(hash, out occurences);
            map[hash] = 1 + (exists ? occurences : 0);
        }

        /// <summary>
        /// Gets the number of occurences for the specified hash code.
        /// </summary>
        /// <param name="hash">The searched hash code.</param>
        /// <returns>The number of occurences found.</returns>
        public int this[int hash]
        {
            get
            {
                int occurences;
                return map.TryGetValue(hash, out occurences) ? occurences : 0;
            }
        }

        /// <summary>
        /// Returns the probability of collision.
        /// </summary>
        /// <returns>The probability of collision between 0 and 1.</returns>
        public double GetCollisionProbability()
        {
            double result = 0d;

            foreach (var pair in map)
            {
                if (pair.Value > 1)
                {
                    double occurences = pair.Value;
                    result += (occurences / count) * ((occurences - 1d) / (count - 1d));
                }
            }

            return result;
        }
    }
}
