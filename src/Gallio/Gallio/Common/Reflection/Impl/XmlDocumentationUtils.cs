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
using System.Text;
using System.Threading;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Helpers for working with <see cref="IXmlDocumentationResolver" />.
    /// </summary>
    public static class XmlDocumentationUtils
    {
        private static IXmlDocumentationResolver resolver;

        /// <summary>
        /// Gets the XML documentation resolver.
        /// </summary>
        public static IXmlDocumentationResolver Resolver
        {
            get
            {
                if (resolver == null)
                    Interlocked.CompareExchange<IXmlDocumentationResolver>(ref resolver, new DefaultXmlDocumentationResolver(), null);
                return resolver;
            }
        }

        /// <summary>
        /// Gets the XML documentation for a member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The XML documentation for the member, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null.</exception>
        public static string GetXmlDocumentation(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            Type type = member as Type ?? member.DeclaringType;
            string assemblyPath = AssemblyUtils.GetAssemblyLocalPath(type.Assembly);
            if (assemblyPath == null)
                return null;

            string memberId = FormatId(member);
            return Resolver.GetXmlDocumentation(assemblyPath, memberId);
        }

        /// <summary>
        /// Gets the XML documentation id of a member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The XML documentation id of the member</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null.</exception>
        public static string FormatId(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return FormatId((MethodBase)member);

                case MemberTypes.Event:
                    return FormatId((EventInfo)member);

                case MemberTypes.Field:
                    return FormatId((FieldInfo)member);

                case MemberTypes.Property:
                    return FormatId((PropertyInfo)member);

                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    return FormatId((Type)member);

                case MemberTypes.Custom:
                default:
                    // Note: XML doc spec doesn't say anything about custom members.
                    //       Using the "!" prefix flags the reference as an error which seems reasonable.
                    StringBuilder str = new StringBuilder(@"!:");
                    AppendType(str, member.DeclaringType, true);
                    str.Append('.');
                    AppendMemberName(str, member);
                    return str.ToString();
            }
        }

        private static string FormatId(Type type)
        {
            StringBuilder str = new StringBuilder(@"T:");
            AppendType(str, type, true);
            return str.ToString();
        }

        private static string FormatId(FieldInfo field)
        {
            StringBuilder str = new StringBuilder(@"F:");
            AppendType(str, field.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, field);
            return str.ToString();
        }

        private static string FormatId(PropertyInfo property)
        {
            // Note: To handle indexers with parameters bound to generic type arguments
            //       of the declaring type, we need to throw away all specialization of the property.
            //       We can do that by locating the same property on the generic type definition
            //       by its metadata token.  The framework doesn't help us out much here since
            //       we can't just resolve the metadata token for a property like we can for methods.
            ParameterInfo[] parameters = property.GetIndexParameters();
            if (parameters.Length != 0 && property.DeclaringType.IsGenericType
                && !property.DeclaringType.IsGenericTypeDefinition)
            {
                Type genericTypeDefn = property.DeclaringType.GetGenericTypeDefinition();
                int desiredMetadataToken = property.MetadataToken;
                Module desiredModule = property.Module;

                PropertyInfo unboundProperty = Array.Find(genericTypeDefn.GetProperties(BindingFlags.DeclaredOnly
                    | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static),
                    delegate(PropertyInfo candidate)
                    {
                        return candidate.Module == desiredModule
                            && candidate.MetadataToken == desiredMetadataToken;
                    });
                    
                if (unboundProperty == null)
                    throw new NotSupportedException(String.Format("Could not resolve property '{0}' that was declared on a generic type.",
                        property));

                property = unboundProperty;
                parameters = property.GetIndexParameters();
            }

            StringBuilder str = new StringBuilder(@"P:");
            AppendType(str, property.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, property);
            AppendParameterList(str, parameters);
            return str.ToString();
        }

        private static string FormatId(EventInfo @event)
        {
            StringBuilder str = new StringBuilder(@"E:");
            AppendType(str, @event.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, @event);
            return str.ToString();
        }

        private static string FormatId(MethodBase method)
        {
            // Note: To handle methods with parameters (or return type in the case of conversion
            //       operators) bound to generic type arguments of the declaring type, we need to
            //       throw away all specialization of the method.  We can do that by resolving
            //       the raw method token.
            if (method.DeclaringType.IsGenericType)
                method = method.Module.ResolveMethod(method.MetadataToken);

            StringBuilder str = new StringBuilder(@"M:");
            AppendType(str, method.DeclaringType, true);
            str.Append('.');
            AppendMemberName(str, method);

            if (method.IsGenericMethod)
            {
                str.Append(@"``");
                str.Append(method.GetGenericArguments().Length);
            }

            AppendParameterList(str, method.GetParameters());

            if (method.IsSpecialName
                && (method.Name == @"op_Implicit" || method.Name == @"op_Explicit"))
            {
                str.Append('~');
                AppendType(str, ((MethodInfo)method).ReturnType, false);
            }

            return str.ToString();
        }

        private static void AppendType(StringBuilder str, Type type, bool useGenericDefinition)
        {
            // TODO: Handle custom type modifiers, pinned types, generic arrays,
            //       ranked arrays, and function pointer types
            if (type.HasElementType)
            {
                Type elementType = type.GetElementType();
                AppendType(str, elementType, useGenericDefinition);

                if (type.IsArray)
                {
                    str.Append(@"[]");
                }
                else if (type.IsByRef)
                {
                    str.Append('@');
                }
                else if (type.IsPointer)
                {
                    str.Append('*');
                }
                else
                {
                    // Don't know what to do with this.
                    str.Append('?');
                }

                return;
            }

            if (type.IsGenericParameter)
            {
                // Generic method parameters use 2 back-ticks instead of 1.
                if (type.DeclaringMethod != null)
                    str.Append('`');
                str.Append('`');
                str.Append(type.GenericParameterPosition);
                return;
            }

            if (type.IsNested)
            {
                AppendType(str, type.DeclaringType, true);
                str.Append('.');
            }
            else
            {
                string @namespace = type.Namespace;
                if (! string.IsNullOrEmpty(@namespace))
                {
                    str.Append(type.Namespace);
                    str.Append('.');
                }
            }

            AppendMemberName(str, type);

            if (type.IsGenericType)
            {
                // Note: The Type's Name already includes `x where x is the number of generic arguments.
                //       So if we just need the id of the generic type definition, we're done.
                //       Otherwise we need to strip off this token and append the concrete types
                //       of the arguments.  That case occurs when closed generic types appear in
                //       method signatures.
                if (!useGenericDefinition && !type.IsGenericTypeDefinition)
                {
                    while (str[str.Length - 1] != '`')
                        str.Length -= 1;

                    str[str.Length - 1] = '{';

                    foreach (Type argument in type.GetGenericArguments())
                    {
                        AppendType(str, argument, false);
                        str.Append(',');
                    }

                    str[str.Length - 1] = '}';
                }
            }
        }

        private static void AppendMemberName(StringBuilder str, MemberInfo member)
        {
            int oldLength = str.Length;
            string memberName = member.Name;

            str.Append(memberName);
            str.Replace('.', '#', oldLength, memberName.Length);
        }

        private static void AppendParameterList(StringBuilder str, ParameterInfo[] parameters)
        {
            if (parameters.Length != 0)
            {
                str.Append('(');

                foreach (ParameterInfo parameter in parameters)
                {
                    AppendType(str, parameter.ParameterType, false);
                    str.Append(',');
                }

                str[str.Length - 1] = ')';
            }
        }
    }
}
