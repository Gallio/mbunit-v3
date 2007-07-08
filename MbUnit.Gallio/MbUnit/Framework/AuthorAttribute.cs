using System;
using MbUnit.Core.Metadata;
using MbUnit.Core.Model;
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