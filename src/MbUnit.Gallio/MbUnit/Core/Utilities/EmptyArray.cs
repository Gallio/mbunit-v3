using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// Provides an empty one dimensional array instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of empty array to produce</typeparam>
    public static class EmptyArray<T>
    {
        /// <summary>
        /// An empty one dimensional array instance of type <typeparamref name="T"/>.
        /// </summary>
        public static readonly T[] Instance = new T[0];
    }
}
