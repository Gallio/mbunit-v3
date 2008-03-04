// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Microsoft.Pex.Framework.Packages;

namespace MbUnit.Pex
{
    /// <summary>
    /// <para>
    /// This attribute enables Pex support for MbUnit.
    /// </para>
    /// <para>
    /// To use MbUnit with Pex, add a reference to this attribute at the assembly-level.
    /// </para>
    /// <example>
    /// <code>
    /// // Enables Pex support for MbUnit.
    /// [assembly: PexMbUnitPackage]
    /// </code>
    /// </example>
    /// </summary>
    public sealed class PexMbUnitPackageAttribute : PexPackageAssemblyAttribute
    {
        /// <summary>
        /// Enables Pex support for MbUnit.
        /// </summary>
        public PexMbUnitPackageAttribute()
            : base(typeof(PexMbUnitPackageAttribute))
        {
        }
    }
}
