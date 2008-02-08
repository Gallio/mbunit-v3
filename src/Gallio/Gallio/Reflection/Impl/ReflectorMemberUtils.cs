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
using Gallio.Reflection;
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for enumerating type memebers and filtering them
    /// based on binding flags.  Takes into account shadowing by member signature.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public static class ReflectorMemberUtils
    {
        private delegate void MemberFlagProvider<T>(T member, out bool isPublic, out bool isStatic, out string signature) where T : IMemberInfo;

        /// <summary>
        /// Provides the members of a particular kind that have been declared by <paramref name="type"/>.
        /// Does not include inherited members.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="type">The type</param>
        /// <returns>The enumeration of members</returns>
        public delegate IEnumerable<T> MemberProvider<T>(ITypeInfo type) where T : IMemberInfo;

        /// <summary>
        /// Finds a member with the specified name within an enumeration of members.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="members">The enumeration of members to search</param>
        /// <param name="memberName">The member name</param>
        /// <returns>The member, or null if none found</returns>
        /// <exception cref="AmbiguousMatchException">Thrown if more than one member
        /// with the specified name is found</exception>
        public static T GetMemberByName<T>(IEnumerable<T> members, string memberName)
            where T : class, IMemberInfo
        {
            T match = null;
            foreach (T member in members)
            {
                if (member.Name == memberName)
                {
                    if (match != null)
                        throw new AmbiguousMatchException(String.Format("Found two matching members named '{0}'.", memberName));

                    match = member;
                }
            }

            return match;
        }

        /// <summary>
        /// Enumerates the fields of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberProvider">The field member provider</param>
        /// <returns>The enumeration of fields</returns>
        public static IEnumerable<IFieldInfo> EnumerateFields(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IFieldInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IFieldInfo field, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = field.IsPublic;
                isStatic = field.IsStatic;
                signature = field.Name;
            });
        }

        /// <summary>
        /// Enumerates the properties of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberProvider">The property member provider</param>
        /// <returns>The enumeration of properties</returns>
        public static IEnumerable<IPropertyInfo> EnumerateProperties(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IPropertyInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IPropertyInfo property, out bool isPublic, out bool isStatic, out string signature)
            {
                IMethodInfo getMethod = property.GetMethod;
                IMethodInfo setMethod = property.SetMethod;

                isPublic = getMethod != null && getMethod.IsPublic
                    || setMethod != null && setMethod.IsPublic;
                isStatic = getMethod != null && getMethod.IsStatic
                    || setMethod != null && setMethod.IsStatic;

                signature = property.Name;
            });
        }

        /// <summary>
        /// Enumerates the events of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberProvider">The event member provider</param>
        /// <returns>The enumeration of events</returns>
        public static IEnumerable<IEventInfo> EnumerateEvents(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IEventInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IEventInfo @event, out bool isPublic, out bool isStatic, out string signature)
            {
                IMethodInfo addMethod = @event.AddMethod;
                IMethodInfo raiseMethod = @event.RaiseMethod;
                IMethodInfo removeMethod = @event.RemoveMethod;

                isPublic = addMethod != null && addMethod.IsPublic
                    || raiseMethod != null && raiseMethod.IsPublic
                        || removeMethod != null && removeMethod.IsPublic;
                isStatic = addMethod != null && addMethod.IsStatic
                    || raiseMethod != null && raiseMethod.IsStatic
                        || removeMethod != null && removeMethod.IsStatic;

                signature = @event.Name;
            });
        }

        /// <summary>
        /// Enumerates the constructors of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberProvider">The constructor member provider</param>
        /// <returns>The enumeration of constructors</returns>
        public static IEnumerable<IConstructorInfo> EnumerateConstructors(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IConstructorInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IConstructorInfo constructor, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = constructor.IsPublic;
                isStatic = constructor.IsStatic;
                signature = null;
            });
        }

        /// <summary>
        /// Enumerates the methods of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberProvider">The method member provider</param>
        /// <returns>The enumeration of methods</returns>
        public static IEnumerable<IMethodInfo> EnumerateMethods(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IMethodInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IMethodInfo method, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = method.IsPublic;
                isStatic = method.IsStatic;

                // Note: The method name will include the number of generic parameters in the case of a generic method.
                StringBuilder signatureBuilder = new StringBuilder(method.Name);

                foreach (IParameterInfo parameter in method.Parameters)
                {
                    signatureBuilder.Append(':');
                    signatureBuilder.Append(parameter.ValueType.AssemblyQualifiedName);
                }

                signature = signatureBuilder.ToString();
            });
        }

        private static IEnumerable<T> EnumerateMembers<T>(ITypeInfo type, BindingFlags bindingFlags,
            MemberProvider<T> memberProvider, MemberFlagProvider<T> flagProvider)
            where T : IMemberInfo
        {
            bool isDeclaringType = true;

            bool publicFlag = (bindingFlags & BindingFlags.Public) != 0;
            bool nonPublicFlag = (bindingFlags & BindingFlags.NonPublic) != 0;
            bool instanceFlag = (bindingFlags & BindingFlags.Instance) != 0;
            bool staticFlag = (bindingFlags & BindingFlags.Static) != 0;
            bool flattenHierarchyFlag = (bindingFlags & BindingFlags.FlattenHierarchy) != 0;
            HashSet<string> signatures = new HashSet<string>();

            foreach (ITypeInfo declaringType in ReflectorInheritanceUtils.EnumerateDeclaringTypes(type, bindingFlags))
            {
                foreach (T member in memberProvider(declaringType))
                {
                    bool isPublic, isStatic;
                    string signature;
                    flagProvider(member, out isPublic, out isStatic, out signature);

                    if (signature == null || !signatures.Contains(signature)) // check for hiding by signature
                    {
                        if ((isPublic && publicFlag || !isPublic && nonPublicFlag)
                            && (isStatic && staticFlag && (isDeclaringType || flattenHierarchyFlag) ||
                                !isStatic && instanceFlag))
                            yield return member;

                        if (signature != null)
                            signatures.Add(signature);
                    }
                }

                isDeclaringType = false;
            }
        }
    }
}