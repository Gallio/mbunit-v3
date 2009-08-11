using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A mirror describes a code element or attribute using a particular reflection policy.
    /// </summary>
    public interface IMirror
    {
        /// <summary>
        /// Gets the reflection policy of the mirror.
        /// </summary>
        IReflectionPolicy ReflectionPolicy { get; }
    }
}
