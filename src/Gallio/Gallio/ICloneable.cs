using System;

namespace Gallio
{
    /// <summary>
    /// Provides a typed clone operation.
    /// </summary>
    /// <typeparam name="T">The type produced when the object is cloned</typeparam>
    public interface ICloneable<T> : ICloneable
        where T : ICloneable<T>
    {
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned object</returns>
        new T Clone();
    }
}
