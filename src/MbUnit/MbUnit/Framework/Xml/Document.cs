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
    /// Represents an XML document.
    /// </summary>
    public class Document : Node, IDiffable<Document>
    {
        private readonly Declaration declaration;

        /// <summary>
        /// Gets the declaration element of the document.
        /// </summary>
        public Declaration Declaration
        {
            get
            {
                return declaration;
            }
        }

        /// <summary>
        /// Constructs an XML document.
        /// </summary>
        /// <param name="declaration">The declaration element of the document.</param>
        /// <param name="root">The root element of the document.</param>
        public Document(Declaration declaration, INode root)
            : base(root)
        {
            if (declaration == null)
                throw new ArgumentNullException("declaration");

            this.declaration = declaration;
        }

        /// <inheritdoc />
        public override string ToXml()
        {
            return declaration.ToXml() + Child.ToXml();
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, Path path, XmlEqualityOptions options)
        {
            return Diff((Document)expected, path, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(Document expected, Path path, XmlEqualityOptions options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");

            return new DiffSetBuilder()
                .Add(declaration.Diff(expected.Declaration, path.Extend("xml", true), options))
                .Add(Child.Diff(expected.Child, path, options))
                .ToDiffSet();
        }
    }
}
