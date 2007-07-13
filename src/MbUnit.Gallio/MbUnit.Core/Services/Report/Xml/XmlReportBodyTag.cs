using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable report body.
    /// Contains all other tags within a report stream.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class XmlReportBodyTag : XmlReportContainerTag
    {
        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        public static XmlReportBodyTag Create()
        {
            XmlReportBodyTag tag = new XmlReportBodyTag();
            tag.Initialize();
            return tag;
        }
    }
}
