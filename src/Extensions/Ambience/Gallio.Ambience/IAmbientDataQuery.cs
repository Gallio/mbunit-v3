using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Represents a lazily evaluated query over Ambient data for use with
    /// the LINQ query syntax.
    /// </para>
    /// </summary>
    public interface IAmbientDataQuery<T> : IEnumerable<T>
    {
    }
}
