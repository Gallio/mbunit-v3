// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates the author's name and email address with a test fixture, test method,
    /// test parameter or other test component.
    /// </summary>
    public class AuthorAttribute : MetadataPatternAttribute
    {
        private string authorName;
        private string authorEmail;

        /// <summary>
        /// Associates the author's name with the test component annotated
        /// by this attribute.
        /// </summary>
        /// <param name="authorName">The author's name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authorName"/> is null</exception>
        public AuthorAttribute(string authorName)
            : this(authorName, "")
        {
        }

        /// <summary>
        /// Associates the author's name and email address with the test component annotated
        /// by this attribute.
        /// </summary>
        /// <param name="authorName">The author's name</param>
        /// <param name="authorEmail">The author's email address</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authorName"/> or <paramref name="authorEmail "/> is null</exception>
        public AuthorAttribute(string authorName, string authorEmail)
        {
            if (authorName == null)
                throw new ArgumentNullException("authorName");
            if (authorEmail == null)
                throw new ArgumentNullException("authorEmail");

            this.authorName = authorName;
            this.authorEmail = authorEmail;
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

        /// <inheritdoc />
        public override void Apply(TestTemplateTreeBuilder builder, ITestComponent component)
        {
            if (authorName.Length != 0)
                component.Metadata.Entries.Add(MetadataConstants.AuthorNameKey, authorName);
            if (authorEmail.Length != 0)
                component.Metadata.Entries.Add(MetadataConstants.AuthorEmailKey, authorEmail);
        }
    }
}