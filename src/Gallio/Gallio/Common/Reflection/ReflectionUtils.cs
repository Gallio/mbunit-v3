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
using System.Reflection;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Provides functions for working with reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets the assembly that declares the code element, or the
        /// code element itself if it is an <see cref="IAssemblyInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null.</param>
        /// <returns>The assembly, or null if not found.</returns>
        public static IAssemblyInfo GetAssembly(ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                return assembly;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Assembly;

            return null;
        }

        /// <summary>
        /// Gets the namespace that declares the code element, or the
        /// code element itself if it is an <see cref="INamespaceInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null.</param>
        /// <returns>The namespace, or null if not found.</returns>
        public static INamespaceInfo GetNamespace(ICodeElementInfo codeElement)
        {
            INamespaceInfo @namespace = codeElement as INamespaceInfo;
            if (@namespace != null)
                return @namespace;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Namespace;

            return null;
        }

        /// <summary>
        /// Gets the type that declares the code element, or the
        /// code element itself if it is an <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null.</param>
        /// <returns>The type, or null if not found.</returns>
        public static ITypeInfo GetType(ICodeElementInfo codeElement)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            if (type != null)
                return type;

            IMemberInfo member = GetMember(codeElement);
            if (member != null)
                return member.DeclaringType;

            return null;
        }

        /// <summary>
        /// Gets the non-type member that declares the code element, or the
        /// code element itself if it is an <see cref="IMemberInfo"/>
        /// other than a <see cref="ITypeInfo" />.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null.</param>
        /// <returns>The member, or null if not found.</returns>
        public static IMemberInfo GetMember(ICodeElementInfo codeElement)
        {
            IMemberInfo member = codeElement as IMemberInfo;
            if (member != null)
                return member is ITypeInfo ? null : member;

            IParameterInfo parameter = GetParameter(codeElement);
            if (parameter != null)
                return parameter.Member;

            return null;
        }

        /// <summary>
        /// Gets the parameter that declares the code element, or the
        /// code element itself if it is an <see cref="IParameterInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null.</param>
        /// <returns>The parameter, or null if not found.</returns>
        public static IParameterInfo GetParameter(ICodeElementInfo codeElement)
        {
            IParameterInfo type = codeElement as IParameterInfo;
            if (type != null)
                return type;

            return null;
        }

        /// <summary>
        /// Finds the assembly name of the directly referenced assembly with the specified display name.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <param name="displayName">The display name of the referenced assembly to find.</param>
        /// <returns>The referenced assembly name or null if none.</returns>
        public static AssemblyName FindAssemblyReference(IAssemblyInfo assembly, string displayName)
        {
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == displayName)
                    return reference;
            }

            return null;
        }

        /// <summary>
        /// Determines if the type can be instantiated using a public constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the type can be instantiated.</returns>
        public static bool CanInstantiate(ITypeInfo type)
        {
            return type != null
                && (type.TypeAttributes & (TypeAttributes.Abstract | TypeAttributes.Class | TypeAttributes.Public)) == (TypeAttributes.Class | TypeAttributes.Public)
                && type.ElementType == null
                && type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Count != 0;
        }

        /// <summary>
        /// Determines if the method is public, non-static and is non-abstract so it can be invoked.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>True if the method can be invoked.</returns>
        public static bool CanInvokeNonStatic(IMethodInfo method)
        {
            return method != null
                && (method.MethodAttributes & (MethodAttributes.Abstract | MethodAttributes.Public | MethodAttributes.Static)) == MethodAttributes.Public;
        }

        /// <summary>
        /// Determines if the property has public, non-static and non-abstract getter
        /// and setter functions.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the property can be get and set.</returns>
        public static bool CanGetAndSetNonStatic(IPropertyInfo property)
        {
            return property != null
                && CanInvokeNonStatic(property.GetMethod)
                && CanInvokeNonStatic(property.SetMethod);
        }

        /// <summary>
        /// Returns true if a type is derived from another type with the specified qualified name.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="qualifiedTypeName">The qualified type name.</param>
        /// <returns>True if <paramref name="type"/> is derived from <paramref name="qualifiedTypeName"/></returns>
        public static bool IsDerivedFrom(ITypeInfo type, string qualifiedTypeName)
        {
            for (ITypeInfo superType = type; superType != null; superType = superType.BaseType)
            {
                if (superType.FullName == qualifiedTypeName)
                    return true;
            }

            foreach (ITypeInfo interfaceType in type.Interfaces)
            {
                if (interfaceType.FullName == qualifiedTypeName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value for the type, such as <c>0</c> if
        /// the type represents an integer, or <c>null</c> if the type
        /// is a reference type or if <paramref name="type"/> was null.</returns>
        public static object GetDefaultValue(Type type)
        {
            return GetDefaultValue(Type.GetTypeCode(type));
        }

        /// <summary>
        /// Gets the default value of a type with a given type code.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        /// <returns>The default value of the type associated with the
        /// specified type code.</returns>
        public static object GetDefaultValue(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return default(Boolean);
                case TypeCode.Byte:
                    return default(Byte);
                case TypeCode.Char:
                    return default(Char);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.Decimal:
                    return default(Decimal);
                case TypeCode.Double:
                    return default(Double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(Int16);
                case TypeCode.Int32:
                    return default(Int32);
                case TypeCode.Int64:
                    return default(Int64);
                case TypeCode.Object:
                    return default(Object);
                case TypeCode.SByte:
                    return default(SByte);
                case TypeCode.Single:
                    return default(Single);
                case TypeCode.String:
                    return default(String);
                case TypeCode.UInt16:
                    return default(UInt16);
                case TypeCode.UInt32:
                    return default(UInt32);
                case TypeCode.UInt64:
                    return default(UInt64);
                default:
                    throw new NotSupportedException("TypeCode not supported.");
            }
        }

        /// <summary>
        /// Returns true if the named type is assignable from the specified type.
        /// </summary>
        /// <param name="searchTypeName">The search type.</param>
        /// <param name="candidateType">The candidate type.</param>
        /// <returns>True if the search type is assignable from the candidate type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchTypeName"/>
        /// or <paramref name="candidateType"/> is null.</exception>
        public static bool IsAssignableFrom(string searchTypeName, Type candidateType)
        {
            if (searchTypeName == null)
                throw new ArgumentNullException("searchTypeName");
            if (candidateType == null)
                throw new ArgumentNullException("candidateType");

            for (Type type = candidateType; type != null; type = type.BaseType)
            {
                if (type.Name == searchTypeName
                    || type.FullName == searchTypeName
                    || type.AssemblyQualifiedName == searchTypeName)
                    return true;
            }

            foreach (Type interfaceType in candidateType.GetInterfaces())
                if (IsAssignableFrom(searchTypeName, interfaceType))
                    return true;

            return false;
        }

        /// <summary>
        /// Workaround for a bug in <see cref="Type.FindMembers"/> which causes it ignore
        /// the binding flags when searching for events.
        /// </summary>
        /// <remarks>
        /// https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=522960
        /// </remarks>
        public static MemberInfo[] FindMembersWorkaround(Type type,
            MemberTypes memberTypes, BindingFlags bindingFlags, MemberFilter memberFilter, object state)
        {
            MemberInfo[] members = type.FindMembers(memberTypes & ~MemberTypes.Event,
                bindingFlags, memberFilter, state);

            if ((memberTypes & MemberTypes.Event) != 0)
            {
                EventInfo[] events = type.GetEvents(bindingFlags);
                int eventCount;
                if (memberFilter != null)
                {
                    eventCount = 0;
                    for (int i = 0; i < events.Length; i++)
                    {
                        if (memberFilter(events[i], state))
                            eventCount += 1;
                        else
                            events[i] = null;
                    }
                }
                else
                {
                    eventCount = events.Length;
                }

                if (eventCount != 0)
                {
                    int index = members.Length;
                    Array.Resize(ref members, index + eventCount);

                    for (int i = 0; i < events.Length; i++)
                    {
                        if (events[i] != null)
                            members[index++] = events[i];
                    }
                }
            }

            return members;
        }
    }
}
