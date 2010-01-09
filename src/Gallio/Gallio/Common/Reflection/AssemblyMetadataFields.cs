// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Specifies the optional fields of the assembly metadata that are to be populated.
    /// </summary>
    [Flags]
    public enum AssemblyMetadataFields
    {
        /// <summary>
        /// Populates the basic PE format information about the assembly.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Populates the assembly name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Populating this field is expensive because it requires loading the entire assembly.
        /// </para>
        /// </remarks>
        /// <seealso cref="AssemblyMetadata.AssemblyName"/>
        AssemblyName = 1,
        
        /// <summary>
        /// Populates the list of assembly references.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Populating this field is expensive because it requires loading the entire assembly.
        /// </para>
        /// </remarks>
        /// <seealso cref="AssemblyMetadata.AssemblyReferences"/>
        AssemblyReferences = 2,

        /// <summary>
        /// Populates the runtime version.
        /// </summary>
        RuntimeVersion = 4
    }
}
