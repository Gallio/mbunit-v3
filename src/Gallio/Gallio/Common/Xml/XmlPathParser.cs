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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Parses the textual representation of an XML path.
    /// </summary>
    internal sealed class XmlPathParser
    {
        /// <summary>
        /// Parses the specified textual representation of a loose path.
        /// </summary>
        /// <param name="input">The input path.</param>
        /// <returns>The resulting path object.</returns>
        public static IXmlPathLoose RunLoose(string input)
        {
            return RunImpl<IXmlPathLoose, IXmlPathLooseClosed, IXmlPathLooseOpen>(input, XmlPathRoot.Empty, 
                (path, id) => path.Element(id),
                (path, id) => path.Attribute(id));
        }

        /// <summary>
        /// Parses the specified textual representation of a strict path.
        /// </summary>
        /// <param name="input">The input path.</param>
        /// <returns>The resulting path object.</returns>
        public static IXmlPathStrict RunStrict(string input)
        {
            return RunImpl<IXmlPathStrict, IXmlPathStrict, IXmlPathStrict>(input, XmlPathRoot.Strict.Empty,
                (path, id) => path.Element(Int32.Parse(id)),
                (path, id) => path.Attribute(Int32.Parse(id)));
        }

        private static T RunImpl<T, TClosed, TOpen>(string input, TOpen root, Func<TOpen, string, TOpen> extendElement, Func<TOpen, string, TClosed> extendAttribute)
            where T : IXmlPath
            where TClosed : T
            where TOpen : TClosed
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.StartsWith(XmlPathRoot.ElementSeparator))
                throw new ArgumentException("Missing root separator.", "input");

            TOpen result = root;

            if (input.Length > 1)
            {
                string[] tokens = input.Substring(1).Split(new[] { XmlPathRoot.ElementSeparator }, StringSplitOptions.None);

                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i].Length == 0)
                        throw new ArgumentException("Invalid path.", "input");

                    string[] subTokens = tokens[i].Split(new[] { XmlPathRoot.AttributeSeparator }, StringSplitOptions.None);

                    if ((subTokens.Length != 1) && (subTokens.Length != 2))
                        throw new ArgumentException("Invalid path.", "input");

                    result = extendElement(result, subTokens[0]);

                    if (subTokens.Length == 2)
                    {
                        if (i != tokens.Length - 1)
                            throw new ArgumentException("Invalid path.", "input");

                        return extendAttribute(result, subTokens[1]);
                    }
                }
            }

            return result;
        }
    }
}
