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
using System.Text;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for constructing names for reflected types and members.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public static class ReflectorNameUtils
    {
        private enum TypeNameStyle
        {
            Short, Full, Signature
        }

        /// <summary>
        /// Creates a valid .Net type name as returned by <see cref="MemberInfo.Name" />.
        /// </summary>
        /// <param name="type">The type for which to generate a name</param>
        /// <param name="simpleName">The simple name of the type without any type annotations, namespaces
        /// or assembly information.  Should be null for a constructed type such as an array.</param>
        /// <returns>The type's name</returns>
        public static string GetTypeName(ITypeInfo type, string simpleName)
        {
            if (type.IsGenericParameter)
                return simpleName;

            return BuildTypeName(type, simpleName, TypeNameStyle.Short);
        }

        /// <summary>
        /// Creates a valid .Net full type name as returned by <see cref="Type.FullName" />.
        /// </summary>
        /// <param name="type">The type for which to generate a full name</param>
        /// <param name="simpleName">The simple name of the type without any type annotations, namespaces
        /// or assembly information.  Should be null for a constructed type such as an array.</param>
        /// <returns>The type's full name, or null for generic parameters</returns>
        public static string GetTypeFullName(ITypeInfo type, string simpleName)
        {
            if (type.IsGenericParameter)
                return null;

            return BuildTypeName(type, simpleName, TypeNameStyle.Full);
        }

        /// <summary>
        /// Creates a valid .Net assembly qualified type name as returned by <see cref="Type.AssemblyQualifiedName" />.
        /// </summary>
        /// <param name="type">The type for which to generate an assembly qualified name</param>
        /// <returns>The type's assembly qualified name, or null for generic parameters</returns>
        public static string GetTypeAssemblyQualifiedName(ITypeInfo type)
        {
            if (type.IsGenericParameter)
                return null;

            return type.FullName + ", " + type.Assembly.FullName;
        }

        /// <summary>
        /// Constructs a type signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="Type" />.  The main difference between the signature and <see cref="Type.FullName" />
        /// is that the signature does not use assembly qualified names.
        /// The format differs slightly in other respects also.
        /// </summary>
        /// <param name="type">The type for which to generate a signature</param>
        /// <param name="simpleName">The simple name of the type without any type annotations, namespaces
        /// or assembly information.  Should be null for a constructed type such as an array.</param>
        /// <returns>The signature</returns>
        public static string GetTypeSignature(ITypeInfo type, string simpleName)
        {
            if (type.IsGenericParameter)
                return simpleName;

            return BuildTypeName(type, simpleName, TypeNameStyle.Signature);
        }

        /// <summary>
        /// Constructs a field signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="FieldInfo" />.  eg. "Int32 field"
        /// </summary>
        /// <param name="field">The field for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetFieldSignature(IFieldInfo field)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(field.ValueType));
            sig.Append(' ');
            sig.Append(field.Name);

            return sig.ToString();
        }

        /// <summary>
        /// Constructs a property signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="PropertyInfo" />.  eg. "Int32 Property" or "Int32 Item [Int32]"
        /// </summary>
        /// <param name="property">The property for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetPropertySignature(IPropertyInfo property)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(property.ValueType));
            sig.Append(' ');
            sig.Append(property.Name);

            IList<IParameterInfo> indexParameters = property.IndexParameters;
            if (indexParameters.Count != 0)
            {
                sig.Append(' ');
                AppendParameterList(sig, indexParameters);
            }

            return sig.ToString();
        }

        /// <summary>
        /// Constructs an event signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="EventInfo" />.  eg. "Int32 DoSomething(System.Object)"
        /// </summary>
        /// <param name="event">The field for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetEventSignature(IEventInfo @event)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(@event.EventHandlerType));
            sig.Append(' ');
            sig.Append(@event.Name);

            return sig.ToString();
        }

        /// <summary>
        /// Constructs a method signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="MethodInfo" />.  eg. "Int32 DoSomething(System.Object)"
        /// </summary>
        /// <param name="method">The method for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetMethodSignature(IMethodInfo method)
        {
            return GetFunctionSignature(method, method.ReturnType, method.GenericArguments);
        }

        /// <summary>
        /// Constructs a constructor signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="ConstructorInfo" />.  eg. "Void .ctor(System.Object)"
        /// </summary>
        /// <param name="constructor">The constructor for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetConstructorSignature(IConstructorInfo constructor)
        {
            return GetFunctionSignature(constructor, Reflector.Wrap(typeof(void)), EmptyArray<ITypeInfo>.Instance);
        }

        private static string GetFunctionSignature(IFunctionInfo function, ITypeInfo returnType, IList<ITypeInfo> genericArguments)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(returnType));
            sig.Append(' ');
            sig.Append(function.Name);
            AppendGenericArgumentList(sig, genericArguments);
            sig.Append('(');
            AppendParameterList(sig, function.Parameters);
            sig.Append(')');

            return sig.ToString();
        }

        /// <summary>
        /// Constructs a parameter signature like .Net produces when calling <see cref="Object.ToString" /> on a
        /// <see cref="ParameterInfo" />.  eg. "Int32 parameter"
        /// </summary>
        /// <param name="parameter">The parameter for which to generate a signature</param>
        /// <returns>The signature</returns>
        public static string GetParameterSignature(IParameterInfo parameter)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(parameter.ValueType));
            sig.Append(' ');
            sig.Append(parameter.Name);

            return sig.ToString();
        }

        private static void AppendParameterList(StringBuilder sig, IList<IParameterInfo> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                    sig.Append(", ");
                sig.Append(GetTypeNameForSignature(parameters[i].ValueType));
            }
        }

        private static void AppendGenericArgumentList(StringBuilder sig, IList<ITypeInfo> genericArguments)
        {
            if (genericArguments.Count != 0)
            {
                sig.Append('[');

                for (int i = 0; i < genericArguments.Count; i++)
                {
                    if (i != 0)
                        sig.Append(',');
                    sig.Append(GetTypeNameForSignature(genericArguments[i]));
                }

                sig.Append(']');
            }
        }

        private static string BuildTypeName(ITypeInfo type, string simpleName, TypeNameStyle style)
        {
            ITypeInfo elementType = type.ElementType;
            if (elementType != null)
            {
                string elementTypeName = GetTypeNameWithStyle(elementType, style);

                if (type.IsPointer)
                    return elementTypeName + "*";

                if (type.IsByRef)
                    return elementTypeName + "&";

                StringBuilder arrayTypeName = new StringBuilder(elementTypeName, elementTypeName.Length + 8);
                arrayTypeName.Append('[');
                arrayTypeName.Append(',', type.ArrayRank - 1);
                arrayTypeName.Append(']');
                return arrayTypeName.ToString();
            }

            StringBuilder typeName = new StringBuilder();

            if (style != TypeNameStyle.Short)
            {
                ITypeInfo declaringType = type.DeclaringType;
                if (declaringType != null)
                {
                    typeName.Append(GetTypeNameWithStyle(declaringType, style));
                    typeName.Append('+');
                }
                else
                {
                    string @namespace = type.Namespace.Name;
                    if (@namespace.Length != 0)
                    {
                        typeName.Append(@namespace);
                        typeName.Append('.');
                    }
                }
            }

            typeName.Append(simpleName);

            IList<ITypeInfo> genericArguments = type.GenericArguments;
            int genericParameterCount = genericArguments.Count;
            if (genericParameterCount != 0)
            {
                typeName.Append('`').Append(genericParameterCount);

                if (style == TypeNameStyle.Full && !type.IsGenericTypeDefinition
                    || style == TypeNameStyle.Signature)
                {
                    typeName.Append('[');

                    for (int i = 0; i < genericArguments.Count; i++)
                    {
                        if (i != 0)
                            typeName.Append(',');

                        ITypeInfo genericArgument = genericArguments[i];
                        if (style == TypeNameStyle.Full)
                        {
                            typeName.Append('[');
                            typeName.Append(genericArgument.AssemblyQualifiedName);
                            typeName.Append(']');
                        }
                        else
                        {
                            typeName.Append(genericArgument.ToString());
                        }
                    }

                    typeName.Append(']');
                }
            }

            return typeName.ToString();
        }

        private static string GetTypeNameWithStyle(ITypeInfo type, TypeNameStyle style)
        {
            switch (style)
            {
                case TypeNameStyle.Short:
                default:
                    return type.Name;

                case TypeNameStyle.Full:
                    return type.FullName;

                case TypeNameStyle.Signature:
                    return type.ToString();
            }
        }

        private static string GetTypeNameForSignature(ITypeInfo type)
        {
            return ReflectorTypeUtils.IsPrimitive(type) ? type.Name : type.ToString();
        }
    }
}
