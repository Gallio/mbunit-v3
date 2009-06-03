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
using System.Reflection;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A <see cref="Type" /> reflection wrapper.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </remarks>
    public interface ITypeInfo : IMemberInfo, IEquatable<ITypeInfo>
    {
        /// <summary>
        /// Gets the assembly in which the type is declared.
        /// </summary>
        IAssemblyInfo Assembly { get; }

        /// <summary>
        /// Gets the namespace in which the type is declared.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the type does not have a namespace, returns an object whose name is an empty string.
        /// This is different from the behavior of <see cref="Type.Namespace" /> which would ordinarily return null.
        /// </para>
        /// </remarks>
        INamespaceInfo Namespace { get; }

        /// <summary>
        /// Gets the name of the namespace in which the type is declared.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the type does not have a namespace, returns an empty string.
        /// This is different from the behavior of <see cref="Type.Namespace" /> which would ordinarily return null.
        /// </para>
        /// </remarks>
        string NamespaceName { get; }

        /// <summary>
        /// Gets the base type of the type, or null if none.
        /// </summary>
        ITypeInfo BaseType { get; }

        /// <summary>
        /// Gets the assembly-qualified name of the type, or null if the type is a generic parameter.
        /// </summary>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// Gets the full name of the type, or null if the type is a generic parameter.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the type attributes.
        /// </summary>
        TypeAttributes TypeAttributes { get; }

        /// <summary>
        /// Returns true if the type is abstract and must be overridden.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Returns true if the type is sealed.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        /// Returns true if the type is a class.
        /// </summary>
        bool IsClass { get; }

        /// <summary>
        /// Returns true if the type is an interface.
        /// </summary>
        bool IsInterface { get; }

        /// <summary>
        /// Returns true if the type represents an enumeration.
        /// </summary>
        bool IsEnum { get; }

        /// <summary>
        /// Returns true if the type is a value type.
        /// </summary>
        bool IsValueType { get; }

        /// <summary>
        /// Returns true if the type is nested.
        /// </summary>
        bool IsNested { get; }

        /// <summary>
        /// Returns true if the type is nested and is visible only within its own assembly.
        /// </summary>
        bool IsNestedAssembly { get; }

        /// <summary>
        /// Returns true if the type is nested and visible only to classes that belong to both its own family and its own assembly.
        /// </summary>
        bool IsNestedFamilyAndAssembly { get; }

        /// <summary>
        /// Returns true if the type is nested and visible only within its own family.
        /// </summary>
        bool IsNestedFamily { get; }

        /// <summary>
        /// Returns true if the type is nested and visible only to classes that belong to either its own family or to its own assembly.
        /// </summary>
        bool IsNestedFamilyOrAssembly { get; }

        /// <summary>
        /// Returns true if the type is nested and declared private.
        /// </summary>
        bool IsNestedPrivate { get; }

        /// <summary>
        /// Returns true if the type is nested and declared public.
        /// </summary>
        bool IsNestedPublic { get; }

        /// <summary>
        /// Returns true if the type is declared public.
        /// </summary>
        bool IsPublic { get; }

        /// <summary>
        /// Returns true if the type is not declared public.
        /// </summary>
        bool IsNotPublic { get; }

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
        /// <seealso cref="IGenericParameterInfo"/>
        bool IsGenericParameter { get; }

        /// <summary>
        /// Returns true if the type is a generic type.
        /// If so, the <see cref="GenericArguments" /> list will be non-empty.
        /// </summary>
        bool IsGenericType { get; }

        /// <summary>
        /// Returns true if the type is a generic type definition.
        /// </summary>
        bool IsGenericTypeDefinition { get; }

        /// <summary>
        /// Returns true if the type contains unbound generic parameters.
        /// If so, the <see cref="GenericArguments" /> list will contain one
        /// or more <see cref="IGenericParameterInfo" /> objects.
        /// </summary>
        bool ContainsGenericParameters { get; }

        /// <summary>
        /// Gets the generic arguments of the type.
        /// The list may contain <see cref="IGenericParameterInfo"/> objects when
        /// no type has yet been bound to a certain generic parameter slots.
        /// </summary>
        /// <returns>The generic arguments, or an empty list if there are none.</returns>
        IList<ITypeInfo> GenericArguments { get; }

        /// <summary>
        /// Gets the generic type definition of this type, or null if the type is not generic.
        /// </summary>
        ITypeInfo GenericTypeDefinition { get; }

        /// <summary>
        /// Gets the rank of the array type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the type is not an array type.</exception>
        int ArrayRank { get; }

        /// <summary>
        /// Gets the type's <see cref="TypeCode" />.
        /// </summary>
        TypeCode TypeCode { get; }

        /// <summary>
        /// Gets all of the type's interfaces.
        /// </summary>
        /// <returns>The type's interfaces.</returns>
        IList<ITypeInfo> Interfaces { get; }

        /// <summary>
        /// Gets the element type of an array, pointer or byref type, or null if none.
        /// </summary>
        ITypeInfo ElementType { get; }

        /// <summary>
        /// Gets all constructors of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The constructors.</returns>
        IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a method by name, or null if not found.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The method.</returns>
        IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all methods of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The methods.</returns>
        IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a property by name, or null if not found.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The property.</returns>
        IPropertyInfo GetProperty(string propertyName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all properties of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The properties.</returns>
        IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a field by name, or null if not found.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The property.</returns>
        IFieldInfo GetField(string fieldName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all fields of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The fields.</returns>
        IList<IFieldInfo> GetFields(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a event by name, or null if not found.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The event.</returns>
        IEventInfo GetEvent(string eventName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all events of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The events.</returns>
        IList<IEventInfo> GetEvents(BindingFlags bindingFlags);

        /// <summary>
        /// Gets a nested type by name, or null if not found.
        /// </summary>
        /// <param name="nestedTypeName">The nested type name.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The nested type.</returns>
        ITypeInfo GetNestedType(string nestedTypeName, BindingFlags bindingFlags);

        /// <summary>
        /// Gets all nested types of the type that satisfy the binding flags.
        /// </summary>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The nested types.</returns>
        IList<ITypeInfo> GetNestedTypes(BindingFlags bindingFlags);

        /// <summary>
        /// Returns true if variables of this type can be assigned with values of the specified type.
        /// </summary>
        /// <param name="type">The other type, or null if none.</param>
        /// <returns>True if the other type is not null and this type is assignable from the other type.</returns>
        bool IsAssignableFrom(ITypeInfo type);

        /// <summary>
        /// Returns true if this type is a subclass of the specified type.
        /// </summary>
        /// <param name="type">The other type, or null if none.</param>
        /// <returns>True if the other type is not null, this type is a subclass of the other type,
        /// and this type not the same as the other type.</returns>
        bool IsSubclassOf(ITypeInfo type);

        /// <summary>
        /// Makes an array type of the specified rank.
        /// </summary>
        /// <param name="arrayRank">The array rank.</param>
        /// <returns>The array type.</returns>
        ITypeInfo MakeArrayType(int arrayRank);

        /// <summary>
        /// Makes a pointer type.
        /// </summary>
        /// <returns>The pointer type.</returns>
        ITypeInfo MakePointerType();

        /// <summary>
        /// Makes a reference type.
        /// </summary>
        /// <returns>The reference type.</returns>
        ITypeInfo MakeByRefType();

        /// <summary>
        /// Makes a generic type instantiation.
        /// </summary>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <returns>The generic type instantiation.</returns>
        ITypeInfo MakeGenericType(IList<ITypeInfo> genericArguments);

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations.</param>
        /// <returns>The native reflection target.</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the target cannot be resolved.</exception>
        new Type Resolve(bool throwOnError);
    }
}
