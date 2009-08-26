using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Categorizes an XML object that can be compared to another 
    /// object of the same type, in order to get the differences.
    /// </summary>
    /// <typeparam name="T">The type of the diffable object.</typeparam>
    public interface IDiffable<T>
    {
        /// <summary>
        /// Diffs the current object with an expected prototype, and returns
        /// a set of differences found.
        /// </summary>
        /// <param name="expected">A prototype representing the expected object.</param>
        /// <param name="path">The path of the parent node.</param>
        /// <param name="options">Comparison options.</param>
        /// <returns></returns>
        DiffSet Diff(T expected, Path path, XmlEqualityOptions options);
    }
}
