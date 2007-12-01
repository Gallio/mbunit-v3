using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Gets the declared element associated with a code element.
    /// </summary>
    public interface IDeclaredElementAccessor
    {
        /// <summary>
        /// Gets the declared element, or null if none.
        /// </summary>
        IDeclaredElement DeclaredElement { get; }
    }
}
