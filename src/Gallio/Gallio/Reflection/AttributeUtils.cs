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
using System.Globalization;

namespace Gallio.Reflection
{
    /// <summary>
    /// Manipulates attributes described by their metadata.
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Gets the attribute of the specified type, or null if none.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="element">The code element</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The attribute, or null if none</returns>
        /// <exception cref="InvalidOperationException">Thrown if the code element
        /// has multiple attributes of the specified type</exception>
        public static T GetAttribute<T>(ICodeElementInfo element, bool inherit) where T : class
        {
            return (T)GetAttribute(element, typeof(T), inherit);
        }

        /// <summary>
        /// Gets the attributes of the specified type.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <returns>The attributes</returns>
        public static IEnumerable<T> GetAttributes<T>(ICodeElementInfo element, bool inherit) where T : class
        {
            foreach (T attrib in element.GetAttributes(Reflector.Wrap(typeof(T)), inherit))
                yield return attrib;
        }

        /// <summary>
        /// Returns true if the code element has an attribute of the specified type.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <returns>True if the code element has at least one attribute of the specified type</returns>
        public static bool HasAttribute<T>(ICodeElementInfo element, bool inherit)
        {
            return HasAttribute(element, typeof(T), inherit);
        }

        /// <summary>
        /// Gets the attribute of the specified type, or null if none.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The attribute, or null if none</returns>
        /// <exception cref="InvalidOperationException">Thrown if the code element
        /// has multiple attributes of the specified type</exception>
        public static object GetAttribute(ICodeElementInfo element, Type attributeType, bool inherit)
        {
            IEnumerator<object> en = element.GetAttributes(Reflector.Wrap(attributeType), inherit).GetEnumerator();
            if (!en.MoveNext())
                return null;

            object attrib = en.Current;

            if (en.MoveNext())
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "There are multiple instances of attribute '{0}'.", attributeType));

            return attrib;
        }

        /// <summary>
        /// Gets the attributes of the specified type.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The attributes</returns>
        public static IEnumerable<object> GetAttributes(ICodeElementInfo element, Type attributeType, bool inherit)
        {
            foreach (object attrib in element.GetAttributes(Reflector.Wrap(attributeType), inherit))
                yield return attrib;
        }

        /// <summary>
        /// Returns true if the code element has an attribute of the specified type.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>True if the code element has at least one attribute of the specified type</returns>
        public static bool HasAttribute(ICodeElementInfo element, Type attributeType, bool inherit)
        {
            return element.HasAttribute(Reflector.Wrap(attributeType), inherit);
        }

        /// <summary>
        /// Resolves all the attributes.
        /// </summary>
        /// <param name="attributes">The attribute descriptions</param>
        /// <returns>The resolved attribute instances</returns>
        public static IEnumerable<object> ResolveAttributes(IEnumerable<IAttributeInfo> attributes)
        {
            foreach (IAttributeInfo attribute in attributes)
                yield return attribute.Resolve();
        }
    }
}
