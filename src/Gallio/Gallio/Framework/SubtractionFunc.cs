using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Framework
{
    /// <summary>
    /// Subtracts the right value from the left and returns the difference.
    /// </summary>
    /// <typeparam name="TValue">The type of values to be compared</typeparam>
    /// <typeparam name="TDifference">The type of the difference produced when the values are
    /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
    /// may differ for other types</typeparam>
    /// <param name="left">The left value</param>
    /// <param name="right">The right value</param>
    /// <returns>The difference when the right value is subtracted from the left</returns>
    internal delegate TDifference SubtractionFunc<TValue, TDifference>(TValue left, TValue right);
}
