using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// An attachment is an embedded object in a report.  An attachment must specify a
    /// content type (a MIME type), and some contents.
    /// </summary>
    [Serializable]
    public abstract class Attachment
    {
        private string name;
        private string contentType;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The name of attachment, or null to automatically assign one</param>
        /// <param name="contentType">The content type, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> is null</exception>
        protected Attachment(string name, string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException("contentType");

            this.name = name == null ? Guid.NewGuid().ToString() : name;
            this.contentType = contentType;
        }

        /// <summary>
        /// Gets the name of the attachment, not null.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the content type of the attachment specified as a MIME type, not null.
        /// <seealso cref="MimeTypes"/> for definitions of common MIME types used by MbUnit.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// Invokes the appropriate visitor method for this attachment type.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public abstract void Accept(IAttachmentVisitor visitor);
    }
}
