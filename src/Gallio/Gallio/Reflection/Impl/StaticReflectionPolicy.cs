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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// The static reflection policy base class is intended to assist with the implementation
    /// of custom reflection policies defined over static metadata.
    /// </para>
    /// <para>
    /// It flattens out the code element hierarchy to ease implementation of new policies.
    /// It provides a mechanism for handling generic type substitutions to ensure a consistent and
    /// correct implementation of generic type and generic method instantiations.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Reflection policy subclasses should not perform generic type substitutions themselves.
    /// The rest of the infrastructure will take care of these details automatically.  However
    /// <see cref="StaticDeclaredTypeWrapper"/> and <see cref="StaticMethodWrapper"/> objects
    /// should be created with the type substitutions of the declaring type extended with
    /// generic type arguments.
    /// </para>
    /// </remarks>
    public abstract class StaticReflectionPolicy : BaseReflectionPolicy
    {
        #region Wrapper Comparisons
        /// <summary>
        /// Determines if two wrappers represent the same object.
        /// </summary>
        /// <param name="a">The first wrapper, not null</param>
        /// <param name="b">The second wrapper, not null</param>
        /// <returns>True if both wrapper represent the same object</returns>
        protected internal virtual bool Equals(StaticWrapper a, StaticWrapper b)
        {
            return a.Handle.Equals(b.Handle);
        }

        /// <summary>
        /// Gets a hashcode for a wrapper.
        /// </summary>
        /// <param name="wrapper">The wrapper, not null</param>
        /// <returns>The wrapper's hash code</returns>
        protected internal virtual int GetHashCode(StaticWrapper wrapper)
        {
            return wrapper.Handle.GetHashCode();
        }
        #endregion

        #region Assemblies
        /// <summary>
        /// Gets the custom attributes of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly, not null</param>
        /// <returns>The attributes</returns>
        protected internal abstract IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets the name of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <returns>The assembly name</returns>
        protected internal abstract AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets the path of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <returns>The assembly path</returns>
        protected internal abstract string GetAssemblyPath(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets the references of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <returns>The assembly references</returns>
        protected internal abstract IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets the public types exported by an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <returns>The types</returns>
        protected internal abstract IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets all types contained in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <returns>The types</returns>
        protected internal abstract IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly);

        /// <summary>
        /// Gets the specified named type within an assembly.
        /// </summary>
        /// <param name="assembly">The assembly wrapper, not null</param>
        /// <param name="typeName">The type name, not null</param>
        /// <returns>The type, or null if none</returns>
        protected internal abstract StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName);
        #endregion

        #region Custom Attributes
        /// <summary>
        /// Gets the constructor of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute, not null</param>
        /// <returns>The constructor</returns>
        protected internal abstract StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute);

        /// <summary>
        /// Gets the constructor arguments of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute, not null</param>
        /// <returns>The constructor argument values</returns>
        protected internal abstract ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute);

        /// <summary>
        /// Gets the field arguments of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute, not null</param>
        /// <returns>The field argument values</returns>
        protected internal abstract IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(StaticAttributeWrapper attribute);

        /// <summary>
        /// Gets the property arguments of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute, not null</param>
        /// <returns>The property argument values</returns>
        protected internal abstract IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(StaticAttributeWrapper attribute);
        #endregion

        #region Member
        /// <summary>
        /// Gets the custom attributes of a member.
        /// </summary>
        /// <param name="member">The member, not null</param>
        /// <returns>The custom attributes</returns>
        protected internal abstract IEnumerable<StaticAttributeWrapper> GetMemberCustomAttributes(StaticMemberWrapper member);

        /// <summary>
        /// Gets the short name of a member.
        /// In the case of a generic type, should exclude the generic parameter count
        /// part of the name.  eg. "`1"
        /// </summary>
        /// <param name="member">The member, not null</param>
        /// <returns>The member's name</returns>
        protected internal abstract string GetMemberName(StaticMemberWrapper member);

        /// <summary>
        /// Gets the source code location of a member.
        /// </summary>
        /// <param name="member">The member, not null</param>
        /// <returns>The source code location, or <see cref="CodeLocation.Unknown" /> if not available</returns>
        protected internal abstract CodeLocation GetMemberSourceLocation(StaticMemberWrapper member);
        #endregion

        #region Events
        /// <summary>
        /// Gets the attributes of an event.
        /// </summary>
        /// <param name="event">The event, not null</param>
        /// <returns>The event attributes</returns>
        protected internal abstract EventAttributes GetEventAttributes(StaticEventWrapper @event);

        /// <summary>
        /// Gets the add method of an event, or null if none.
        /// </summary>
        /// <param name="event">The event, not null</param>
        /// <returns>The add method, or null if none</returns>
        protected internal abstract StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event);

        /// <summary>
        /// Gets the raise method of an event, or null if none.
        /// </summary>
        /// <param name="event">The event, not null</param>
        /// <returns>The raise method, or null if none</returns>
        protected internal abstract StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event);

        /// <summary>
        /// Gets the remove method of an event, or null if none.
        /// </summary>
        /// <param name="event">The event, not null</param>
        /// <returns>The remove method, or null if none</returns>
        protected internal abstract StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event);

        /// <summary>
        /// Gets the event handler type of an event.
        /// </summary>
        /// <param name="event">The event, not null</param>
        /// <returns>The event handler type</returns>
        protected internal abstract StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event);
        #endregion

        #region Fields
        /// <summary>
        /// Gets the attributes of a field.
        /// </summary>
        /// <param name="field">The field, not null</param>
        /// <returns>The field attributes</returns>
        protected internal abstract FieldAttributes GetFieldAttributes(StaticFieldWrapper field);

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <param name="field">The field, not null</param>
        /// <returns>The field type</returns>
        protected internal abstract StaticTypeWrapper GetFieldType(StaticFieldWrapper field);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the attributes of a property.
        /// </summary>
        /// <param name="property">The property, not null</param>
        /// <returns>The property attributes</returns>
        protected internal abstract PropertyAttributes GetPropertyAttributes(StaticPropertyWrapper property);

        /// <summary>
        /// Gets the property type.
        /// </summary>
        /// <param name="property">The property, not null</param>
        /// <returns>The property type</returns>
        protected internal abstract StaticTypeWrapper GetPropertyType(StaticPropertyWrapper property);

        /// <summary>
        /// Gets the get method of a property, or null if none.
        /// </summary>
        /// <param name="property">The property, not null</param>
        /// <returns>The get method, or null if none</returns>
        protected internal abstract StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property);

        /// <summary>
        /// Gets the set method of a property, or null if none.
        /// </summary>
        /// <param name="property">The property, not null</param>
        /// <returns>The set method, or null if none</returns>
        protected internal abstract StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property);
        #endregion

        #region Functions
        /// <summary>
        /// Gets the attributes of a function.
        /// </summary>
        /// <param name="function">The function, not null</param>
        /// <returns>The function attributes</returns>
        protected internal abstract MethodAttributes GetFunctionAttributes(StaticFunctionWrapper function);

        /// <summary>
        /// Gets the calling conventions of a function.
        /// </summary>
        /// <param name="function">The function, not null</param>
        /// <returns>The function calling conventions</returns>
        protected internal abstract CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function);

        /// <summary>
        /// Gets the parameters of a function.
        /// </summary>
        /// <param name="function">The function, not null</param>
        /// <returns>The parameters</returns>
        protected internal abstract IList<StaticParameterWrapper> GetFunctionParameters(StaticFunctionWrapper function);
        #endregion

        #region Constructors
        #endregion

        #region Methods
        /// <summary>
        /// Gets the generic parameters of a method.
        /// </summary>
        /// <param name="method">The method, not null</param>
        /// <returns>The generic parameters</returns>
        protected internal abstract IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method);

        /// <summary>
        /// Gets the return parameter of a method.
        /// </summary>
        /// <param name="method">The method, not null</param>
        /// <returns>The return parameter</returns>
        protected internal abstract StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method);
        #endregion

        #region Parameters
        /// <summary>
        /// Gets the attributes of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter, not null</param>
        /// <returns>The parameter attributes</returns>
        protected internal abstract ParameterAttributes GetParameterAttributes(StaticParameterWrapper parameter);

        /// <summary>
        /// Gets the custom attributes of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter, not null</param>
        /// <returns>The custom attributes</returns>
        protected internal abstract IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter);

        /// <summary>
        /// Gets the name of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter, not null</param>
        /// <returns>The parameter's name</returns>
        protected internal abstract string GetParameterName(StaticParameterWrapper parameter);

        /// <summary>
        /// Gets the parameter's position, or -1 if the parameter is a return value.
        /// </summary>
        /// <param name="parameter">The parameter, not null</param>
        /// <returns>The parameter's position</returns>
        protected internal abstract int GetParameterPosition(StaticParameterWrapper parameter);

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        /// <param name="parameter">The parameter, not null</param>
        /// <returns>The parameter type</returns>
        protected internal abstract StaticTypeWrapper GetParameterType(StaticParameterWrapper parameter);
        #endregion

        #region Declared Types
        /// <summary>
        /// Gets the attributes of a type.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type attributes</returns>
        protected internal abstract TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the assembly that contains a type.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's assembly</returns>
        protected internal abstract StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the namespace that contains a type.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's namespace, or an empty string if it has none</returns>
        protected internal abstract string GetTypeNamespace(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the base type of atype.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The base type</returns>
        protected internal abstract StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the interfaces directly implemented by a type.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's interfaces</returns>
        protected internal abstract IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the generic parameters of a type.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's generic parameters</returns>
        protected internal abstract IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the constructors of a type.
        /// Only includes declared methods, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's constructors</returns>
        protected internal abstract IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type);

        /// <summary>
        /// Gets the methods of a type including accessor methods for properties and events.
        /// Only includes declared methods, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <param name="reflectedType">The reflected type, not null</param>
        /// <returns>The type's methods</returns>
        protected internal abstract IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType);

        /// <summary>
        /// Gets the properties of a type.
        /// Only includes declared methods, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <param name="reflectedType">The reflected type, not null</param>
        /// <returns>The type's properties</returns>
        protected internal abstract IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType);

        /// <summary>
        /// Gets the fields of a type.
        /// Only includes declared methods, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <param name="reflectedType">The reflected type, not null</param>
        /// <returns>The type's fields</returns>
        protected internal abstract IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType);

        /// <summary>
        /// Gets the events of a type.
        /// Only includes declared methods, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <param name="reflectedType">The reflected type, not null</param>
        /// <returns>The type's events</returns>
        protected internal abstract IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType);

        /// <summary>
        /// Gets the nested types of a type.
        /// Only includes declared nested types, not inherited ones.
        /// </summary>
        /// <param name="type">The type, not null</param>
        /// <returns>The type's nested types</returns>
        protected internal abstract IEnumerable<StaticTypeWrapper> GetTypeNestedTypes(StaticDeclaredTypeWrapper type);
        #endregion

        #region Generic Parameters
        /// <summary>
        /// Gets the attributes of a generic parameter.
        /// </summary>
        /// <param name="genericParameter">The generic parameter, not null</param>
        /// <returns>The generic parameter attributes</returns>
        protected internal abstract GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter);

        /// <summary>
        /// Gets the generic parameter position.
        /// </summary>
        /// <param name="genericParameter">The generic parameter, not null</param>
        /// <returns>The generic parameter position</returns>
        protected internal abstract int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter);

        /// <summary>
        /// Gets the generic parameter constraints.
        /// </summary>
        /// <param name="genericParameter">The generic parameter, not null</param>
        /// <returns>The generic parameter constraints</returns>
        protected internal abstract IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter);
        #endregion
    }
}
