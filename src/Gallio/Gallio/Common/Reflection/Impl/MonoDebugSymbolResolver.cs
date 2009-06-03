// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Implementation of a debug symbol resolver for Mono.
    /// </summary>
    public class MonoDebugSymbolResolver : IDebugSymbolResolver
    {
        /// <summary>
        /// Creates a Mono debug symbol resolver.
        /// </summary>
        /// <param name="avoidLocks">If true, avoids taking a lock on the PDB files but may use more memory or storage.</param>
        public MonoDebugSymbolResolver(bool avoidLocks)
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public CodeLocation GetSourceLocationForMethod(string assemblyPath, int methodToken)
        {
            // TODO
            return CodeLocation.Unknown;
        }
    }
}
