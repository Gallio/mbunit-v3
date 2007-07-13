using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// Represents a text-encoded attachment.
    /// </summary>
    [Serializable]
    public class TextAttachment : Attachment
    {
        private string text;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="text">The text string, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="text"/> is null</exception>
        public TextAttachment(string name, string contentType, string text)
            : base(name, contentType)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
        }

        /// <summary>
        /// Gets the text of the attachment, not null.
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <inheritdoc />
        public override void Accept(IAttachmentVisitor visitor)
        {
            visitor.VisitTextAttachment(this);
        }
    }
}
