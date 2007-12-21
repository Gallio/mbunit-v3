// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model.Reflection;
using JetBrains.Util;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Provides support for filtering type members based on binding flags.
    /// Takes into account the inheritance structure of the type and member
    /// shadowing by signature.
    /// </summary>
    internal static class ReSharperBinder
    {
        public delegate IEnumerable<T> MemberProvider<T>(ITypeInfo type) where T : IMemberInfo;
        private delegate void FlagProvider<T>(T member, out bool isPublic, out bool isStatic, out string signature) where T : IMemberInfo;

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

        public static IEnumerable<IFieldInfo> EnumerateFields(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IFieldInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IFieldInfo field, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = field.IsPublic;
                isStatic = field.IsStatic;
                signature = field.Name;
            });
        }

        public static IEnumerable<IPropertyInfo> EnumerateProperties(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IPropertyInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IPropertyInfo property, out bool isPublic, out bool isStatic, out string signature)
            {
                IMethodInfo getMethod = property.GetGetMethod();
                IMethodInfo setMethod = property.GetSetMethod();

                isPublic = getMethod != null && getMethod.IsPublic
                    || setMethod != null && setMethod.IsPublic;
                isStatic = getMethod != null && getMethod.IsStatic
                    || setMethod != null && setMethod.IsStatic;

                signature = property.Name;
            });
        }

        public static IEnumerable<IEventInfo> EnumerateEvents(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IEventInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IEventInfo @event, out bool isPublic, out bool isStatic, out string signature)
            {
                IMethodInfo addMethod = @event.GetAddMethod();
                IMethodInfo raiseMethod = @event.GetRaiseMethod();
                IMethodInfo removeMethod = @event.GetRemoveMethod();

                isPublic = addMethod != null && addMethod.IsPublic
                    || raiseMethod != null && raiseMethod.IsPublic
                    || removeMethod != null && removeMethod.IsPublic;
                isStatic = addMethod != null && addMethod.IsStatic
                    || raiseMethod != null && raiseMethod.IsStatic
                    || removeMethod != null && removeMethod.IsStatic;

                signature = @event.Name;
            });
        }

        public static IEnumerable<IConstructorInfo> EnumerateConstructors(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IConstructorInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IConstructorInfo constructor, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = constructor.IsPublic;
                isStatic = constructor.IsStatic;
                signature = null;
            });
        }

        public static IEnumerable<IMethodInfo> EnumerateMethods(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<IMethodInfo> memberProvider)
        {
            return EnumerateMembers(type, bindingFlags, memberProvider, delegate(IMethodInfo method, out bool isPublic, out bool isStatic, out string signature)
            {
                isPublic = method.IsPublic;
                isStatic = method.IsStatic;

                StringBuilder signatureBuilder = new StringBuilder(method.Name);
                
                foreach (IGenericParameterInfo genericParameter in method.GetGenericParameters())
                {
                    signatureBuilder.Append(':');
                    signatureBuilder.Append(genericParameter.Name);
                }

                foreach (IParameterInfo parameter in method.GetParameters())
                {
                    signatureBuilder.Append(':');
                    signatureBuilder.Append(parameter.Name);
                }

                signature = signatureBuilder.ToString();
            });
        }

        private static IEnumerable<T> EnumerateMembers<T>(ITypeInfo type, BindingFlags bindingFlags, MemberProvider<T> memberProvider, FlagProvider<T> flagProvider)
            where T : IMemberInfo
        {
            bool isDeclaringType = true;

            bool publicFlag = (bindingFlags & BindingFlags.Public) != 0;
            bool nonPublicFlag = (bindingFlags & BindingFlags.NonPublic) != 0;
            bool instanceFlag = (bindingFlags & BindingFlags.Instance) != 0;
            bool staticFlag = (bindingFlags & BindingFlags.Static) != 0;
            bool flattenHierarchyFlag = (bindingFlags & BindingFlags.FlattenHierarchy) != 0;
            HashSet<string> signatures = new HashSet<string>();

            foreach (ITypeInfo declaringType in EnumerateDeclaringTypes(type, bindingFlags))
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

        private static IEnumerable<ITypeInfo> EnumerateDeclaringTypes(ITypeInfo type, BindingFlags bindingFlags)
        {
            yield return type;

            if ((bindingFlags & BindingFlags.DeclaredOnly) == 0)
            {
                if ((type.TypeAttributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class)
                {
                    for (ITypeInfo baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                        yield return baseType;
                }
                else
                {
                    HashSet<ITypeInfo> interfaces = new HashSet<ITypeInfo>();
                    PopulateSuperInterfaces(type, interfaces);

                    foreach (ITypeInfo @interface in interfaces)
                        yield return @interface;
                }
            }
        }

        private static void PopulateSuperInterfaces(ITypeInfo type, HashSet<ITypeInfo> interfaces)
        {
            foreach (ITypeInfo @interface in type.GetInterfaces())
            {
                interfaces.Add(type);
                PopulateSuperInterfaces(@interface, interfaces);
            }

            for (ITypeInfo baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                interfaces.Add(baseType);
                PopulateSuperInterfaces(baseType, interfaces);
            }
        }
    }
}
