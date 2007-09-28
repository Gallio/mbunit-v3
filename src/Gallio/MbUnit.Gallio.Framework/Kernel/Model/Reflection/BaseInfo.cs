using System;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// Abstract base class for read-only reflection model objects.
    /// </summary>
    public abstract class BaseInfo
    {
        private readonly object source;

        /// <summary>
        /// Copies the contents of a source model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        internal BaseInfo(object source)
        {
            if (source == null)
                throw new ArgumentNullException(@"source");

            this.source = source;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseInfo);
        }

        /// <summary>
        /// Compares this object's source for equality with the other's source.
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True if the objects are equal</returns>
        public bool Equals(BaseInfo other)
        {
            return other != null && source.Equals(other.source);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return source.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return source.ToString();
        }

        /// <summary>
        /// Gets the model object wrapped by this instance.
        /// </summary>
        internal object Source
        {
            get { return source; }
        }
    }
}
