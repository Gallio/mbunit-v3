using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// Specifies the Xml-encoding of an embedded report attachment.
    /// </summary>
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public enum XmlContentEncoding
    {
        /// <summary>
        /// The attachment is encoded as a text string.
        /// </summary>
        [XmlEnum("text")]
        Text,

        /// <summary>
        /// The attachment is encoded in XML as child elements.
        /// </summary>
        [XmlEnum("xml")]
        Xml,

        /// <summary>
        /// The attachment is encoded as base 64 text string.
        /// </summary>
        [XmlEnum("base64")]
        Base64
    }
}
