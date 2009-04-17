using System;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Specifies the result of an attempt to resolve a dependency.
    /// </summary>
    public struct DependencyResolution
    {
        private readonly bool isSatisfied;
        private readonly object value;

        private DependencyResolution(bool isSatisfied, object value)
        {
            this.isSatisfied = isSatisfied;
            this.value = value;
        }

        /// <summary>
        /// Creates a result object that represents a satisfied dependency.
        /// </summary>
        /// <param name="value">The value provided for the dependency</param>
        /// <returns>The result object</returns>
        public static DependencyResolution Satisfied(object value)
        {
            return new DependencyResolution(true, value);
        }

        /// <summary>
        /// Creates a result object that represents an unsatisfied dependency.
        /// </summary>
        /// <returns>The result object</returns>
        public static DependencyResolution Unsatisfied()
        {
            return new DependencyResolution(false, null);
        }

        /// <summary>
        /// Returns true if the dependency was satisfied.
        /// </summary>
        public bool IsSatisfied
        {
            get { return isSatisfied; }
        }

        /// <summary>
        /// Returns the value used to satisfy the dependency.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the dependency was not satisfied</exception>
        public object Value
        {
            get
            {
                if (!isSatisfied)
                    throw new InvalidOperationException("The dependency was not satisfied.");
                return value;
            }
        }
    }
}