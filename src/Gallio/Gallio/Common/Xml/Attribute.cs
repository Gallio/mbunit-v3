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
    /// Represents a single attribute in an XML element.
    /// </summary>
    public class Attribute : IDiffable<Attribute>, INamed
    {
        private readonly string name;
        private readonly string value;

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Constructs an attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public Attribute(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Returns the XML fragment for the attribute.
        /// </summary>
        /// <returns>The resulting XML fragment representing the attribute.</returns>
        public string ToXml()
        {
            return String.Format("{0}=\"{1}\"", name, value);
        }

        /// <inheritdoc />
        public DiffSet Diff(Attribute expected, IXmlPathOpen path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            var builder = new DiffSetBuilder();

            if (!AreNamesEqual(expected.Name, options))
            {
                builder.Add(new Diff(path.ToString(), "Unexpected attribute found.", expected.Name, name));
            }
            else 
            {
                if (!value.Equals(expected.Value, GetComparisonTypeForValue(options)))
                {
                    builder.Add(new Diff(path.Attribute(name).ToString(), "Unexpected attribute value found.", expected.Value, value));
                }
            }

            return builder.ToDiffSet();
        }

        private static StringComparison GetComparisonTypeForName(Options options)
        {
            return (((options & Options.IgnoreAttributesNameCase) != 0) 
                ? StringComparison.CurrentCultureIgnoreCase 
                : StringComparison.CurrentCulture);
        }

        private static StringComparison GetComparisonTypeForValue(Options options)
        {
            return (((options & Options.IgnoreAttributesValueCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        /// <inheritdoc />
        public bool AreNamesEqual(string otherName, Options options)
        {
            return name.Equals(otherName, GetComparisonTypeForName(options));
        }

        /// <summary>
        /// Determines whether the value of the current attribute is equal to the specified value, 
        /// by respecting the specified equality options.
        /// </summary>
        /// <param name="otherValue">The value to compare.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>True if the values are equal; false otherwise.</returns>
        public bool AreValuesEqual(string otherValue, Options options)
        {
            return Value.Equals(otherValue, GetComparisonTypeForValue(options));
        }
    }
}
