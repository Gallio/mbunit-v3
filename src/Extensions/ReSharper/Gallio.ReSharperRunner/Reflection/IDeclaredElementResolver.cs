using System;
using System.Collections.Generic;
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
