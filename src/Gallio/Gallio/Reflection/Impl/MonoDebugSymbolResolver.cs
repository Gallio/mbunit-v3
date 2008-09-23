using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Implementation of a debug symbol resolver for Mono.
    /// </summary>
    public class MonoDebugSymbolResolver : IDebugSymbolResolver
    {
        /// <inheritdoc />
        public CodeLocation GetSourceLocationForMethod(string assemblyPath, int methodToken)
        {
            // TODO
            return CodeLocation.Unknown;
        }
    }
}
