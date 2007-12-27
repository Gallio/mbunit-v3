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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for traversing the inheritance structure of
    /// members and types.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public static class ReflectorInheritanceUtils
    {
        /// <summary>
        /// Enumerates the inherited declarations of a property from its declaring supertypes.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The enumeration of inherited property declarations</returns>
        public static IEnumerable<IPropertyInfo> EnumerateSuperProperties(IPropertyInfo property)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the inherited declarations of an event from its declaring supertypes.
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>The enumeration of inherited event declarations</returns>
        public static IEnumerable<IEventInfo> EnumerateSuperEvents(IEventInfo @event)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the inherited declarations of a method from its declaring supertypes.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The enumeration of inherited method declarations</returns>
        public static IEnumerable<IMethodInfo> EnumerateSuperMethods(IMethodInfo method)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the inherited declarations of a parameter from its declaring
        /// method's declaring supertypes.
        /// </summary>
        /// <param name="parameter">The method parameter</param>
        /// <returns>The enumeration of inherited parameter declarations</returns>
        public static IEnumerable<IParameterInfo> EnumerateSuperParameters(IParameterInfo parameter)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the supertypes of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The enumeration of supertypes</returns>
        public static IEnumerable<ITypeInfo> EnumerateSuperTypes(ITypeInfo type)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates all types that could declare members that satisfy the binding flags
        /// proceeding from subtypes up to supertypes.  In particular, looks at the
        /// <see cref="BindingFlags.DeclaredOnly" /> flag to determine whether supertypes
        /// should be included in the enumeration.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <returns>The enumeration of declaring types</returns>
        public static IEnumerable<ITypeInfo> EnumerateDeclaringTypes(ITypeInfo type, BindingFlags bindingFlags)
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
            foreach (ITypeInfo @interface in type.Interfaces)
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