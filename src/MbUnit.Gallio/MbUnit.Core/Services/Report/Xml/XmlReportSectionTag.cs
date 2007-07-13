using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable container for a section of a report with
    /// an identifying section name.  This tag is used to delineate
    /// sections within a report.
    /// </summary>
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReportSectionTag : XmlReportContainerTag
    {
        private string name;

        /// <summary>
        /// Gets or sets the section name, not null.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="sectionName">The section name</param>
        public static XmlReportSectionTag Create(string sectionName)
        {
            XmlReportSectionTag tag = new XmlReportSectionTag();
            tag.Initialize();
            tag.name = sectionName;
            return tag;
        }
    }
}
