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
using Gallio.Common.Xml;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Entry-point class that serves as builder for an XML path.
    /// </summary>
    public static class XmlPathRoot
    {
        /// <summary>
        /// Empty path instance.
        /// </summary>
        internal static readonly IXmlPathLooseOpen Empty = XmlPathLooseOpenElement.Root;

        /// <summary>
        /// Starts a new loose XML path by specifying the name of its root element.
        /// </summary>
        /// <param name="name">The name of the root element.</param>
        /// <returns>A new open loose XML path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        public static IXmlPathLooseOpen Element(string name)
        {
            return Empty.Element(name);
        }

        /// <summary>
        /// Parses the specified textual representation of a path.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The specified path should start with a slash character ('/').
        /// Elements are separated by a slash, and attributes denoted by a colon (':').
        /// </para>
        /// <example><![CDATA[
        /// "/Root/Parent/Child:someAttribute"
        /// ]]></example>
        /// </remarks>
        /// <param name="path">The textual representation of a path.</param>
        /// <returns>The resulting path object.</returns>
        public static IXmlPathLoose Parse(string path)
        {
            return XmlPathParser.RunLoose(path);
        }

        /// <summary>
        /// Separator for elements.
        /// </summary>
        internal static readonly string ElementSeparator = "/";

        /// <summary>
        /// Separator for elements.
        /// </summary>
        internal static readonly string AttributeSeparator = ":"; 

        /// <summary>
        /// Entry-point for strict XML paths.
        /// </summary>
        internal static class Strict
        {
            /// <summary>
            /// Empty path instance.
            /// </summary>
            internal static readonly IXmlPathStrict Empty = XmlPathStrictElement.Root;

            /// <summary>
            /// Starts a new strict XML path by specifying the index of its root element.
            /// </summary>
            /// <param name="index">The index of the root element.</param>
            /// <returns>A new open loose XML path.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative.</exception>
            internal static IXmlPathStrict Element(int index)
            {
                return Empty.Element(index);
            }

            /// <summary>
            /// Parses the specified textual representation of a path.
            /// </summary>
            /// <remarks>
            /// <para>
            /// The specified path should start with a slash character ('/').
            /// Elements are separated by a slash, and attributes denoted by a colon (':').
            /// </para>
            /// <example><![CDATA[
            /// "/Root/Parent/Child:someAttribute"
            /// ]]></example>
            /// </remarks>
            /// <param name="path">The textual representation of a path.</param>
            /// <returns>The resulting path object.</returns>
            public static IXmlPathStrict Parse(string path)
            {
                return XmlPathParser.RunStrict(path);
            }
        }
    }
}