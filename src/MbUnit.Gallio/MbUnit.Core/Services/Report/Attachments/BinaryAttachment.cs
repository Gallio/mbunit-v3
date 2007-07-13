using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MbUnit.Core.Services.Report.Xml;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Services.Report.Attachments
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
        public override XmlReportAttachment XmlSerialize()
        {
            return XmlReportAttachment.Create(Name, ContentType, XmlContentEncoding.Base64,
                Convert.ToBase64String(data, Base64FormattingOptions.None),
                EmptyArray<XmlElement>.Instance);
        }
    }
}
