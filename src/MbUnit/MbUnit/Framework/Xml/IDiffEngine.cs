using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Diffing engine.
    /// </summary>
    /// <typeparam name="T">The type of the objects to diff.</typeparam>
    public interface IDiffEngine<T>
        where T : IDiffable<T>
    {
        /// <summary>
        /// Computes the differences between the expected and the actual object.
        /// </summary>
        /// <returns>The resulting diff set.</returns>
        DiffSet Diff();
    }
}
