using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An Xml-serializable container for a section of a report with
    /// an identifying section header.  This tag is used to delineate
    /// sections within a report.
    /// </summary>
    [XmlType]
    [Serializable]
    public sealed class XmlReportSectionTag : XmlReportContainerTag
    {
        private string heading = "";

        /// <summary>
        /// Gets or sets the section heading, not null.
        /// </summary>
        [XmlAttribute("heading")]
        public string Heading
        {
            get { return heading; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                heading = value;
            }
        }
    }
}
