// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Reflection;

namespace Gallio.Reflection
{
    /// <summary>
    /// Describes the kind of code element specified by a <see cref="CodeReference" />.
    /// </summary>
    public enum CodeReferenceKind
    {
        /// <summary>
        /// The code reference is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The code reference specifies an assembly.
        /// </summary>
        Assembly,

        /// <summary>
        /// The code reference specifies a namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// The code reference specifies a type.
        /// </summary>
        Type,

        /// <summary>
        /// The code reference specifies a constructor, method, property, field or event.
        /// </summary>
        Member,

        /// <summary>
        /// The code reference specifies a parameter of a constructor or method.
        /// </summary>
        Parameter
    }
}