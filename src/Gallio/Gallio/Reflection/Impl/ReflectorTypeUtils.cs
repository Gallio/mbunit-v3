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
using Gallio.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for working with types and type codes.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public class ReflectorTypeUtils
    {
        /// <summary>
        /// Determines the type code of a reflected type based on the
        /// type's name.
        /// </summary>
        /// <param name="type">The reflected type, or null if none</param>
        /// <returns>The type code of the type, or <see cref="TypeCode.Empty" />
        /// if <paramref name="type"/> was null</returns>
        public static TypeCode GetTypeCode(ITypeInfo type)
        {
            if (type == null)
                return TypeCode.Empty;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return TypeCode.Boolean;
                case "System.Byte":
                    return TypeCode.Byte;
                case "System.Char":
                    return TypeCode.Char;
                case "System.DateTime":
                    return TypeCode.DateTime;
                case "System.DBNull":
                    return TypeCode.DBNull;
                case "System.Decimal":
                    return TypeCode.Decimal;
                case "System.Double":
                    return TypeCode.Double;
                case "System.Int16":
                    return TypeCode.Int16;
                case "System.Int32":
                    return TypeCode.Int32;
                case "System.Int64":
                    return TypeCode.Int64;
                case "System.SByte":
                    return TypeCode.SByte;
                case "System.Single":
                    return TypeCode.Single;
                case "System.String":
                    return TypeCode.String;
                case "System.UInt16":
                    return TypeCode.UInt16;
                case "System.UInt32":
                    return TypeCode.UInt32;
                case "System.UInt64":
                    return TypeCode.UInt64;
                default:
                    return TypeCode.Object;
            }
        }

        /// <summary>
        /// Gets the default value for a reflected type.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <returns>The default value for the type, such as <c>0</c> if
        /// the type represents an integer, or <c>null</c> if the type
        /// is a reference type or if <paramref name="type"/> was null</returns>
        public static object GetDefaultValue(ITypeInfo type)
        {
            if (type == null)
                return null;

            return ReflectionUtils.GetDefaultValue(type.TypeCode);
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
            return BuildTypeName(type, simpleName, false);
        }

        /// <summary>
        /// Creates a valid .Net full type name as returned by <see cref="Type.FullName" />.
        /// </summary>
        /// <param name="type">The type for which to generate a full name</param>
        /// <param name="simpleName">The simple name of the type without any type annotations, namespaces
        /// or assembly information.  Should be null for a constructed type such as an array.</param>
        /// <returns>The type's full name</returns>
        public static string GetTypeFullName(ITypeInfo type, string simpleName)
        {
            return BuildTypeName(type, simpleName, true);
        }

        private static string BuildTypeName(ITypeInfo type, string simpleName, bool isFullName)
        {
            if (type.IsGenericParameter)
                return simpleName;

            ITypeInfo elementType = type.ElementType;
            if (elementType != null)
            {
                string elementTypeName = isFullName ? elementType.FullName : elementType.Name;

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

            if (isFullName)
            {
                ITypeInfo declaringType = type.DeclaringType;
                if (declaringType != null)
                {
                    typeName.Append(declaringType.FullName);
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

            int genericParameterCount = type.GenericArguments.Count;
            if (genericParameterCount != 0)
                typeName.Append('`').Append(genericParameterCount);

            if (isFullName && ! type.IsGenericTypeDefinition)
            {
                IList<ITypeInfo> genericArguments = type.GenericArguments;
                if (genericArguments.Count != 0)
                {
                    typeName.Append('[');

                    for (int i = 0; i < genericArguments.Count; i++ )
                    {
                        if (i != 0)
                            typeName.Append(',');

                        typeName.Append('[');
                        typeName.Append(genericArguments[i].AssemblyQualifiedName);
                        typeName.Append(']');
                    }

                    typeName.Append(']');
                }
            }

            return typeName.ToString();
        }

        /// <summary>
        /// Creates a valid .Net assembly qualified type name as returned by <see cref="Type.AssemblyQualifiedName" />.
        /// </summary>
        /// <param name="type">The type for which to generate an assembly qualified name</param>
        /// <returns>The type's assembly qualified name</returns>
        public static string GetTypeAssemblyQualifierName(ITypeInfo type)
        {
            if (type.IsGenericParameter)
                return type.Name;

            if (type.Assembly == null)
                GC.KeepAlive(null);

            return type.FullName + ", " + type.Assembly.FullName;
        }
    }
}