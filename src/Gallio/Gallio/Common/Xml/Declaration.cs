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
    /// Represents the declaration element at the beginning of an XML document.
    /// </summary>
    public class Declaration : IDiffable<Declaration>
    {
        private readonly AttributeCollection attributes;

        /// <summary>
        /// Gets the declaration attributes.
        /// </summary>
        public AttributeCollection Attributes
        {
            get
            {
                return attributes;
            }
        }

        /// <summary>
        /// Constructs the declaration element.
        /// </summary>
        /// <param name="attributes">The attributes of the element.</param>
        public Declaration(AttributeCollection attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            this.attributes = attributes;
        }

        /// <summary>
        /// Returns the XML fragment for the declaration element.
        /// </summary>
        /// <returns>The resulting XML fragment representing the declaration element.</returns>
        public string ToXml()
        {
            var output = attributes.ToXml();

            if (output.Length > 0)
                output = String.Format("<?xml{0}?>", output);

            return output;
        }

        /// <inheritdoc />
        public DiffSet Diff(Declaration expected, IXmlPathOpen path, Options options)
        {
            return attributes.Diff(expected.Attributes, path, options);
        }
    }
}
