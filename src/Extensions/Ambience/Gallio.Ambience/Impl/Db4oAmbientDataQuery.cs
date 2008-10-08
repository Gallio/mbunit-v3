using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Linq;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// Facade over <see cref="IDb4oLinqQuery{T}" />.
    /// </summary>
    /// <typeparam name="T">The query result type</typeparam>
    internal class Db4oAmbientDataQuery<T> : IAmbientDataQuery<T>
    {
        private readonly IDb4oLinqQuery<T> inner;

        /// <summary>
        /// Creates a wrapper for a Db4o query.
        /// </summary>
        /// <param name="inner">The inner query</param>
        public Db4oAmbientDataQuery(IDb4oLinqQuery<T> inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <summary>
        /// Gets the Db4o query object.
        /// </summary>
        public IDb4oLinqQuery<T> Inner
        {
            get { return inner; }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
