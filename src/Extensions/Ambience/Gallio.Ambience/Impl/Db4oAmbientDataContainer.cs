using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// Facade over <see cref="IObjectContainer" />.
    /// </summary>
    internal class Db4oAmbientDataContainer : IAmbientDataContainer
    {
        private readonly IObjectContainer inner;

        /// <summary>
        /// Creates a wrapper for a Db4o object container.
        /// </summary>
        /// <param name="inner">The inner container</param>
        public Db4oAmbientDataContainer(IObjectContainer inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <summary>
        /// Gets the Db4o object container.
        /// </summary>
        public IObjectContainer Inner
        {
            get { return inner; }
        }

        /// <inheritdoc />
        public IList<T> Query<T>()
        {
            return inner.Query<T>();
        }

        /// <inheritdoc />
        public IList<T> Query<T>(Predicate<T> predicate)
        {
            return inner.Query<T>(predicate);
        }

        /// <inheritdoc />
        public void Delete(object obj)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Store(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
