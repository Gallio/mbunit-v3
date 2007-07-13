using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// Represents a binary-encoded attachments.
    /// </summary>
    [Serializable]
    public class BinaryAttachment : Attachment
    {
        private byte[] data;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The attachment name, not null</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="data">The binary data, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="data"/> is null</exception>
        public BinaryAttachment(string name, string contentType, byte[] data)
            : base(name, contentType)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;
        }

        /// <summary>
        /// Gets the binary content of the attachment, not null.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        /// <inheritdoc />
        public override void Accept(IAttachmentVisitor visitor)
        {
            visitor.VisitBinaryAttachment(this);
        }
    }
}
