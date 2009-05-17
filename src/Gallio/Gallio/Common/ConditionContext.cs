using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common
{
    /// <summary>
    /// A condition context provides information used to evaluate whether a condition holds true.
    /// </summary>
    public abstract class ConditionContext
    {
        /// <summary>
        /// Returns true if the context contains a value for a given property.
        /// </summary>
        /// <param name="namespace">The property namespace</param>
        /// <param name="identifier">The property identifier</param>
        /// <returns>True if the context has the specified property</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespace"/>
        /// or <paramref name="identifier"/> is null</exception>
        public bool HasProperty(string @namespace, string identifier)
        {
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            return HasPropertyImpl(@namespace, identifier);
        }

        /// <summary>
        /// Returns true if the context contains a value for a given property.
        /// </summary>
        /// <param name="namespace">The property namespace, not null</param>
        /// <param name="identifier">The property identifier, not null</param>
        /// <returns>True if the context has the specified property</returns>
        protected abstract bool HasPropertyImpl(string @namespace, string identifier);
    }
}