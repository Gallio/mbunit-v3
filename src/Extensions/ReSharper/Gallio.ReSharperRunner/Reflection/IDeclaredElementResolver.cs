using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Resolves a declared element.
    /// </summary>
    public interface IDeclaredElementResolver
    {
        /// <summary>
        /// Resolves the declared element.
        /// </summary>
        /// <returns>The declared element</returns>
        IDeclaredElement ResolveDeclaredElement();
    }
}
