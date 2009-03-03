using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Collections
{
    /// <summary>
    /// An equality comparer that compares values by reference.
    /// </summary>
    /// <typeparam name="T">The type of values to compare, must be reference types</typeparam>
    public sealed class ReferentialEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private static ReferentialEqualityComparer<T> instance = new ReferentialEqualityComparer<T>();

        private ReferentialEqualityComparer()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static ReferentialEqualityComparer<T> Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(T obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
