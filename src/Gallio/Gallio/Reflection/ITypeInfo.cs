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
using System.Collections.Generic;
using System.Reflection;

namespace Gallio.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="Type" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface ITypeInfo : IMemberInfo, IEquatable<ITypeInfo>
    {
        /// <summary>
        /// Gets the assembly in which the type is declared.
        /// </summary>
        IAssemblyInfo Assembly { get; }

        /// <summary>
        /// Gets the namespace in which the type is declared.
        /// </summary>
        INamespaceInfo Namespace { get; }

        /// <summary>
        /// Gets the base type of the type, or null if none.
        /// </summary>
        ITypeInfo BaseType { get; }

        /// <summary>
        /// Gets the assembly-qualified name of the type.
        /// </summary>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the type attributes.
        /// </summary>
        TypeAttributes TypeAttributes { get; }

        /// <summary>
        /// Gets the element type of a constructed type such as an array or pointer, or null if none.
        /// </summary>
        ITypeInfo ElementType { get; }

        /// <summary>
        /// Returns true if the type represents an array.
        /// </summary>
        bool IsArray { get; }

        /// <summary>
        /// Returns true if the type represents a pointer.
        /// </summary>
        bool IsPointer { get; }

        /// <summary>
        /// Returns true if the type represents a type that is passed by reference.
        /// </summary>
        bool IsByRef { get; }

        /// <summary>
        /// Returns true if the type represents a generic type parameter.
        /// </summary>
        bool IsGenericParameter { get; }

        /// <summary>
        /// Returns true if the type is a generic type definition.
        /// </summary>
        bool IsGenericTypeDefinition { get; }

        /// <summary>
        /// Gets the rank of the array type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the type is not an array type</exception>
        int ArrayRank { get; }

        /// <summary>
        /// Gets the type's <see cref="TypeCode" />.
        /// </summary>
        TypeCode TypeCode { get; }

        /// <summary>
        /// Gets all of the type's interfaces.
        /// </summary>
        /// <returns>The type's interfaces</returns>
        IList<ITypeInfo> Interfaces { get; }

        /// <summary>
        /// Gets the generic parameters of the type.
        /// </summary>
        /// <returns>The generic parameters</returns>
        IList<IGenericParameterInfo> GenericParameters { get; }

        /// <summary>
        /// Gets all constructors of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The constructors</returns>
        IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a method by name, or null if not found.
        /// </summary>
        /// <param name="methodName">The method name</param>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The method</returns>
        IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all methods of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The methods</returns>
        IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);

        /// <summary>
        /// Gets all properties of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The properties</returns>
        IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);

        /// <summary>
        /// Gets all fields of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The fields</returns>
        IList<IFieldInfo> GetFields(BindingFlags bindingFlags);

        /// <summary>
        /// Gets all events of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The method binding flags</param>
        /// <returns>The events</returns>
        IList<IEventInfo> GetEvents(BindingFlags bindingFlags);

        /// <summary>
        /// Returns true if variables of this type can be assigned with values of the specified type.
        /// </summary>
        /// <param name="type">The other type</param>
        /// <returns>True if this type is assignable from the other type</returns>
        bool IsAssignableFrom(ITypeInfo type);

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="CodeElementResolveException">Thrown if the target cannot be resolved</exception>
        new Type Resolve(bool throwOnError);
    }
}
