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
using System.Collections.Generic;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Provides information about the contents of a custom attribute.
    /// </summary>
    public interface IAttributeInfo : IMirror
    {
        /// <summary>
        /// Gets the attribute type.
        /// </summary>
        ITypeInfo Type { get; }

        /// <summary>
        /// Gets the constructor used to create the attribute.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if not supported.</exception>
        IConstructorInfo Constructor { get; }

        /// <summary>
        /// Gets an attribute field value.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>The value, or a default value of the field's type if the field with the specified name
        /// was not initialized by the attribute declaration.</returns>
        /// <exception cref="ArgumentException">Thrown if there is no writable instance field with the specified name.</exception>
        ConstantValue GetFieldValue(string name);

        /// <summary>
        /// Gets an attribute property value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The value, or a default value of the propery's type if the property with the specified name was not
        /// initialized by the attribute declaration.</returns>
        /// <exception cref="ArgumentException">Thrown if there is no writable instance property with the specified name.</exception>
        ConstantValue GetPropertyValue(string name);

        /// <summary>
        /// Gets the initialized attribute constructor argument values.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if not supported.</exception>
        ConstantValue[] InitializedArgumentValues { get; }

        /// <summary>
        /// Gets the initialized attribute field values.
        /// </summary>
        IEnumerable<KeyValuePair<IFieldInfo, ConstantValue>> InitializedFieldValues { get; }

        /// <summary>
        /// Gets the initialized attribute property values.
        /// </summary>
        IEnumerable<KeyValuePair<IPropertyInfo, ConstantValue>> InitializedPropertyValues { get; }

        /// <summary>
        /// Gets the attribute as an object.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise the result may include unresolved types, enums or arrays
        /// though it still may throw an exception if the attribute class cannot be instantiated.</param>
        /// <returns>The attribute.</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the attribute could not be resolved.</exception>
        object Resolve(bool throwOnError);
    }
}
