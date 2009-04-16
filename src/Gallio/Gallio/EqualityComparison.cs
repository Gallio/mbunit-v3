using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio
{
    /// <summary>
    /// Represents the method that determines whether two objects of the same type are equal.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>True if the object are equal; otherwise false.</returns>
    public delegate bool EqualityComparison<T>(T x, T y);
}
