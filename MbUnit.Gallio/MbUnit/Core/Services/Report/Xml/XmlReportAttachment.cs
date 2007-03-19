using System;
using System.Xml;
using System.Xml.Serialization;
using MbUnit.Core.Services.Report.Attachments;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An Xml-serializable representation of a report attachment.
    /// The attachment is encoded as needed for transport via Xml.
    /// 
    /// When serialized to Xml, the contents of the attachment are encoded
    /// and embedded in the report according to their encoding.  Text and Xml markup
    /// are directly embedded into the Xml report whereas binary attachments are base 64 encoded.
    /// 
    /// Conversions to/from Xml are performed lazily and cached.
    /// </summary>
    [XmlType]
    public class XmlReportAttachment
    {
        private Attachment attachment;

        private string name = "";
        private string contentType = "";
        private XmlContentEncoding contentEncoding;
        private string innerText = "";
        private XmlElement[] innerXml = EmptyArray<XmlElement>.Instance;

        /// <summary>
        /// Gets or sets the unencoded report attachment, not null.
        /// Setting this property overrides the value of all of the others.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlIgnore]
        public Attachment Attachment
        {
            get
            {
                if (attachment == null)
                {
                    attachment = Attachment.XmlDeserializeFrom(this);
                }

                return attachment;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                value.XmlSerializeTo(this);
                attachment = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the attachment, not null.
        /// Setting this property overrides the value of <see cref="Attachment" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                attachment = null;
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type of the attachment as a MIME type, not null.
        /// Setting this property overrides the value of <see cref="Attachment" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("contentType")]
        public string ContentType
        {
            get
            { 
                return contentType;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                attachment = null;
                contentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the content encoding of the attachment.
        /// This value specifies how the attachment is embedded in Xml.
        /// Setting this property overrides the value of <see cref="Attachment" />.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is not a valid content encoding</exception>
        [XmlAttribute("contentEncoding")]
        public XmlContentEncoding ContentEncoding
        {
            get
            {
                return contentEncoding;
            }
            set
            {
                if (Enum.IsDefined(typeof(XmlContentEncoding), value))
                    throw new ArgumentOutOfRangeException("value", value, "Value is not a valid content encoding.");

                attachment = null;
                contentEncoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the attachment content serialized as text (including Base64 attachments), not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlText]
        public string InnerText
        {
            get
            {
                return innerText;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                attachment = null;
                innerText = value;
            }
        }

        /// <summary>
        /// Gets or sets the attachment content serialized as Xml, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAnyElement]
        public XmlElement[] InnerXml
        {
            get
            {
                return innerXml;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                attachment = null;
                innerXml = value;
            }
        }
    }
}
