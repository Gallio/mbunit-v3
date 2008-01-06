// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Resolves debug symbols associated with members.
    /// </summary>
    public interface IDebugSymbolResolver
    {
        /// <summary>
        /// Gets the location of a source file that contains the declaration of a method, or
        /// null if not available.
        /// </summary>
        /// <param name="assemblyPath">The path of the assembly that contains the method</param>
        /// <param name="methodToken">The method token</param>
        /// <returns>The source code location, or null if unknown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyPath"/> is null</exception>
        CodeLocation GetSourceLocationForMethod(string assemblyPath, int methodToken);
    }
}