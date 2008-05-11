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

namespace Gallio.Reflection
{
    /// <summary>
    /// Describes the kind of code element represented by a <see cref="ICodeElementInfo"/>.
    /// </summary>
    public enum CodeElementKind
    {
        /// <summary>
        /// The element is an <see cref="IAssemblyInfo" />.
        /// </summary>
        Assembly,

        /// <summary>
        /// The element is an <see cref="INamespaceInfo" />.
        /// </summary>
        Namespace,

        /// <summary>
        /// The element is an <see cref="ITypeInfo" />.
        /// </summary>
        Type,

        /// <summary>
        /// The element is an <see cref="IFieldInfo" />.
        /// </summary>
        Field,

        /// <summary>
        /// The element is an <see cref="IPropertyInfo" />.
        /// </summary>
        Property,

        /// <summary>
        /// The element is an <see cref="IEventInfo" />.
        /// </summary>
        Event,

        /// <summary>
        /// The element is an <see cref="Constructor" />.
        /// </summary>
        Constructor,

        /// <summary>
        /// The element is an <see cref="IMethodInfo" />.
        /// </summary>
        Method,

        /// <summary>
        /// The element is an <see cref="IParameterInfo" />.
        /// </summary>
        Parameter,

        /// <summary>
        /// The element is an <see cref="IGenericParameterInfo" />.
        /// </summary>
        GenericParameter
    }
}
