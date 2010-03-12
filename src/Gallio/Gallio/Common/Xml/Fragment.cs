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
using Gallio.Common.Xml;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents a diffable XML fragment.
    /// </summary>
    public sealed class Fragment : MarkupBase, IDiffable<Fragment>
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="declaration"/> is null.</exception>
        public Fragment(Declaration declaration, IMarkup root)
            : base(0, new[] { root })
        {
            if (declaration == null)
                throw new ArgumentNullException("declaration");
            if (root == null)
                throw new ArgumentNullException("root");

            this.declaration = declaration;
        }

        /// <inheritdoc />
        public override DiffSet Diff(IMarkup expected, IXmlPathStrict path, Options options)
        {
            var fragment = expected as Fragment;

            if (fragment == null)
                throw new ArgumentException("Invalid XML fragment", "expected");
                
            return Diff(fragment, path, options);
        }

        /// <summary>
        /// Diffs the current fragment with an expected prototype, and returns a set of differences found.
        /// </summary>
        /// <param name="expected">A prototype representing the expected content of the markup.</param>
        /// <param name="options">Comparison options.</param>
        /// <returns>The resulting diff set describing the differences found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expected"/> is null.</exception>
        public DiffSet Diff(Fragment expected, Options options)
        {
            return Diff(expected, XmlPathRoot.Strict.Empty, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(Fragment expected, IXmlPathStrict path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            return new DiffSetBuilder()
                .Add(declaration.Diff(expected.Declaration, path.Declaration(), options))
                .Add(Children.Diff(expected.Children, path, options))
                .ToDiffSet();
        }

        /// <summary>
        /// Finds the markup located at the specified path.
        /// </summary>
        /// <param name="path">The searched strict path.</param>
        /// <returns>The markup found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        public IMarkup Find(IXmlPathStrict path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            IMarkup current = this;

            foreach (IXmlPathStrict item in path.ToArray())
            {
                current = item.FindInParent(current);
            }

            return current;
        }

        /// <inheritdoc />
        public override int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            return Children.CountAt(searchedPath, expectedValue, options);
        }
    }
}
