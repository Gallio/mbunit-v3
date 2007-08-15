using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// Determines whether two values satisfy a binary relation and
    /// returns true if they do.  Examples of relations are equality,
    /// inequality, less-than, greater-than, and any other well-defined
    /// function that operates on two values yielding true or false.
    /// </summary>
    /// <typeparam name="T">The type of values to compare</typeparam>
    /// <param name="x">The first value</param>
    /// <param name="y">The second value</param>
    /// <returns>True if the values satisfy the relation</returns>
    public delegate bool Relation<T>(T x, T y);
}
