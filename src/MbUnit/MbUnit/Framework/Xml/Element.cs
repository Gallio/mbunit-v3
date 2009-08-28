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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Represents an element in an XML fragment.
    /// </summary>
    public class Element : Node, IDiffable<Element>, INamed
    {
        private readonly string name;
        private readonly string value;
        private readonly AttributeCollection attributes;

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the literal value of the element.
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        public AttributeCollection Attributes
        {
            get
            {
                return attributes;
            }
        }

        /// <summary>
        /// Constructs an XML element.
        /// </summary>
        /// <param name="child">The child node of the element (usually an <see cref="Element"/> or an <see cref="ElementCollection"/>)</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <param name="attributes">The attributes of the element.</param>
        public Element(INode child, string name, string value, AttributeCollection attributes)
            : base(child)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            this.name = name;
            this.value = value;
            this.attributes = attributes;
        }

        /// <inheritdoc />
        public override string ToXml()
        {
            if (!Child.IsNull)
            {
                return String.Format("<{0}{1}>{2}</{0}>", name, attributes.ToXml(), Child.ToXml());
            }
            else if (value.Length == 0)
            {
                return String.Format("<{0}{1}/>", name, attributes.ToXml());
            }
            else
            {
                return String.Format("<{0}{1}>{2}</{0}>", name, attributes.ToXml(), value);
            }
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, Path path, XmlEqualityOptions options)
        {
            return Diff((Element)expected, path, options);
        }

        /// <inheritdoc />
        public virtual DiffSet Diff(Element expected, Path path, XmlEqualityOptions options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            var builder = new DiffSetBuilder();

            if (!AreNamesEqual(expected.Name, options))
            {
                builder.Add(new Diff(path.ToString(), "Unexpected element found.", expected.Name, name));
            }
            else
            {
                if (!value.Equals(expected.Value, GetComparisonTypeForValue(options)))
                {
                    builder.Add(new Diff(path.Extend(name).ToString(), "Unexpected element value found.", expected.Value, value));
                }

                builder.Add(attributes.Diff(expected.Attributes, path.Extend(name), options));
                builder.Add(Child.Diff(expected.Child, path.Extend(name), options));
            }

            return builder.ToDiffSet();
        }

        private static StringComparison GetComparisonTypeForName(XmlEqualityOptions options)
        {
            return (((options & XmlEqualityOptions.IgnoreElementsNameCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        private static StringComparison GetComparisonTypeForValue(XmlEqualityOptions options)
        {
            return (((options & XmlEqualityOptions.IgnoreElementsValueCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        /// <inheritdoc />
        public bool AreNamesEqual(string otherName, XmlEqualityOptions options)
        {
            return name.Equals(otherName, GetComparisonTypeForName(options));
        }
    }
}
