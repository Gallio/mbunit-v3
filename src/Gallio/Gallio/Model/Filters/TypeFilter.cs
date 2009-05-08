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
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestComponent.CodeElement" />
    /// matches the specified type name.
    /// </summary>
    /// <remarks>
    /// Generic types should be specified by the name of their generic type definition.
    /// eg. Foo`1.
    /// </remarks>
    [Serializable]
    public class TypeFilter<T> : PropertyFilter<T> where T : ITestComponent
    {
        private readonly bool includeDerivedTypes;

        /// <summary>
        /// Creates a type filter.
        /// </summary>
        /// <param name="typeNameFilter">A filter to match the type name obtained via reflection
        /// on the type in one of the following ways:
        /// <list type="bullet">
        /// <item>Fully qualified by namespace and assembly as returned by <see cref="Type.AssemblyQualifiedName" /></item>
        /// <item>Qualified by namespace as returned by <see cref="Type.FullName" /></item>
        /// <item>Unqualified as returned by the type's <see cref="MemberInfo.Name" /></item>
        /// </list>
        /// </param>
        /// <param name="includeDerivedTypes">If true, subclasses and interface implementations of the specified
        /// type are also matched by the filter if they can be located</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeNameFilter"/> is null</exception>
        public TypeFilter(Filter<string> typeNameFilter, bool includeDerivedTypes)
            : base(typeNameFilter)
        {
            this.includeDerivedTypes = includeDerivedTypes;
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return includeDerivedTypes ? @"Type" : @"ExactType"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            ITypeInfo type = ReflectionUtils.GetType(value.CodeElement);
            if (type == null)
                return false;

            if (IsMatchForType(type))
                return true;

            if (includeDerivedTypes)
            {
                for (ITypeInfo baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                {
                    if (IsMatchForType(baseType))
                        return true;
                }

                foreach (ITypeInfo @interface in type.Interfaces)
                    if (IsMatchForType(@interface))
                        return true;
            }

            return false;
        }

        private bool IsMatchForType(ITypeInfo type)
        {
            type = type.GenericTypeDefinition ?? type;

            return IsMatchForTypeName(type.AssemblyQualifiedName)
                || IsMatchForTypeName(type.FullName)
                || IsMatchForTypeName(type.Name);
        }

        private bool IsMatchForTypeName(string name)
        {
            if (ValueFilter.IsMatch(name))
                return true;

            return name.Contains("+") && ValueFilter.IsMatch(name.Replace('+', '.'));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Type(" + ValueFilter + @", " + includeDerivedTypes + @")";
        }
    }
}