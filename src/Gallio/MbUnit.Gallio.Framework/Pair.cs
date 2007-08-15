using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// An immutable structure that contains two values.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public struct Pair<TFirst, TSecond>
    {
        private TFirst first;
        private TSecond second;

        /// <summary>
        /// Creates a pair.
        /// </summary>
        /// <param name="first">The first value</param>
        /// <param name="second">The second value</param>
        public Pair(TFirst first, TSecond second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        public TFirst First
        {
            get { return first; }
        }

        /// <summary>
        /// Gets the second value.
        /// </summary>
        public TSecond Second
        {
            get { return second; }
        }
    }
}
