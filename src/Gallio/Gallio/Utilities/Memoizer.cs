using System;

namespace Gallio.Utilities
{
    /// <summary>
    /// A structure that memoizes the result of some computation for later reuse.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public struct Memoizer<T>
    {
        private T value;
        private bool isPopulated;

        /// <summary>
        /// Gets the memoized value if available, otherwise populates it
        /// using the specified populator function and stores it for later reuse.
        /// </summary>
        /// <param name="populator">The populator</param>
        /// <returns>The value returned by the populator, possibly memoized</returns>
        public T Memoize(Func<T> populator)
        {
            if (!isPopulated)
            {
                value = populator();
                isPopulated = true;
            }

            return value;
        }
    }
}
