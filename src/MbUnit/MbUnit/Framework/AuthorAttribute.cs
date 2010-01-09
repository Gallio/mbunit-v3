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
using Gallio.Model;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates the author's name and email address with a test fixture, test method,
    /// test parameter or other test component.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = true, Inherited = true)]
    public class AuthorAttribute : MetadataPatternAttribute
    {
        private readonly string authorName;
        private readonly string authorEmail;
        private readonly string authorHomepage;

        /// <summary>
        /// Associates the author's name with the test component annotated
        /// by this attribute.
        /// </summary>
        /// <param name="authorName">The author's name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authorName"/> is null.</exception>
        public AuthorAttribute(string authorName)
            : this(authorName, "", "")
        {
        }

        /// <summary>
        /// Associates the author's name and email address with the test component annotated
        /// by this attribute.
        /// </summary>
        /// <param name="authorName">The author's name.</param>
        /// <param name="authorEmail">The author's email address.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authorName"/> or <paramref name="authorEmail "/> is null.</exception>
        public AuthorAttribute(string authorName, string authorEmail)
            : this(authorName, authorEmail, "")
        {
        }

        /// <summary>
        /// Associates the author's name, email address and homepage with the test component annotated
        /// by this attribute.
        /// </summary>
        /// <param name="authorName">The author's name.</param>
        /// <param name="authorEmail">The author's email address.</param>
        /// <param name="authorHomepage">The author's home page.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authorName"/>,
        /// <paramref name="authorEmail "/> or <paramref name="authorHomepage"/> is null.</exception>
        public AuthorAttribute(string authorName, string authorEmail, string authorHomepage)
        {
            if (authorName == null)
                throw new ArgumentNullException("authorName");
            if (authorEmail == null)
                throw new ArgumentNullException("authorEmail");
            if (authorHomepage == null)
                throw new ArgumentNullException("authorHomepage");

            this.authorName = authorName;
            this.authorEmail = authorEmail;
            this.authorHomepage = authorHomepage;
        }

        /// <summary>
        /// Gets or sets the author's name.
        /// </summary>
        public string AuthorName
        {
            get { return authorName; }
        }

        /// <summary>
        /// Gets or sets the author's email address or an empty string if none.
        /// </summary>
        public string AuthorEmail
        {
            get { return authorEmail; }
        }

        /// <summary>
        /// Gets or sets the author's homepage or an empty string if none.
        /// </summary>
        public string AuthorHomepage
        {
            get { return authorHomepage; }
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (authorName.Length != 0)
                yield return new KeyValuePair<string, string>(MetadataKeys.AuthorName, authorName);
            if (authorEmail.Length != 0)
                yield return new KeyValuePair<string, string>(MetadataKeys.AuthorEmail, authorEmail);
            if (authorHomepage.Length != 0)
                yield return new KeyValuePair<string, string>(MetadataKeys.AuthorHomepage, authorHomepage);
        }
    }
}