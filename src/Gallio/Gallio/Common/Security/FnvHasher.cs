using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Security
{
    /// <summary>
    /// Calculates a 32-bit hash code based on the FNV (Fowler-Noll-Vo) algorithm.
    /// (http://en.wikipedia.org/wiki/Fowler_Noll_Vo_hash)
    /// </summary>
    public class FnvHasher
    {
        private const int Initial = -2128831035;
        private const int Vector = 16777619;
        private int value;

        /// <summary>
        /// Returns the final hash code.
        /// </summary>
        /// <returns>The resulting 32-bit hash code.</returns>
        public virtual int ToValue()
        {
            return value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FnvHasher()
        {
            value = Initial;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="salt">Salt value.</param>
        public FnvHasher(int salt)
            : this()
        {
            Combine(salt);
        }

        /// <summary>
        /// Adds the hash code of the specified object and combines it with the previous values.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <returns>The builder itself for chaining syntax.</returns>
        public FnvHasher Add(object obj)
        {
            Combine(obj);
            return this;
        }

        /// <summary>
        /// Adds the hash code of the specified objects and combines them with the previous values.
        /// </summary>
        /// <param name="objects">An enumeration of objects to add.</param>
        /// <returns>The builder itself for chaining syntax.</returns>
        public FnvHasher Add(IEnumerable objects)
        {
            if (!ReferenceEquals(null, objects))
            {
                foreach (object obj in objects)
                    Combine(obj);
            }

            return this;
        }

        private void Combine(object obj)
        {
            value = (value * Vector) ^ (obj ?? 0).GetHashCode();
        }
    }
}
