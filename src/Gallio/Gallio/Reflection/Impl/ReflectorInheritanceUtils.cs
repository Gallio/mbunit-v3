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

using System.Collections.Generic;
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
        public static IEnumerable<ICodeElementInfo> EnumerateSuperProperties(IPropertyInfo property)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the inherited declarations of an event from its declaring supertypes.
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>The enumeration of inherited event declarations</returns>
        public static IEnumerable<ICodeElementInfo> EnumerateSuperEvents(IEventInfo @event)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the inherited declarations of a method from its declaring supertypes.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The enumeration of inherited method declarations</returns>
        public static IEnumerable<ICodeElementInfo> EnumerateSuperMethods(IMethodInfo method)
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
        public static IEnumerable<ICodeElementInfo> EnumerateSuperParameters(IParameterInfo parameter)
        {
            // TODO
            yield break;
        }

        /// <summary>
        /// Enumerates the supertypes of a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The enumeration of supertypes</returns>
        public static IEnumerable<ICodeElementInfo> EnumerateSuperTypes(ITypeInfo type)
        {
            for (ITypeInfo baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                yield return baseType;
        }
    }
}