using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// Abstract class of Xml-serializable report container tags.
    /// </summary>
    public abstract class XmlReportContainerTag : XmlReportTag
    {
        private List<XmlReportTag> contents;

        /// <summary>
        /// Gets or sets the nested contents of this tag.
        /// </summary>
        [XmlArray("contents", IsNullable=false)]
        [XmlArrayItem("section", typeof(XmlReportSectionTag), IsNullable = false)]
        [XmlArrayItem("text", typeof(XmlReportTextTag), IsNullable = false)]
        [XmlArrayItem("embed", typeof(XmlReportEmbedTag), IsNullable = false)]
        public XmlReportTag[] Contents
        {
            get { return contents.ToArray(); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                contents = new List<XmlReportTag>(value);
            }
        }

        /// <summary>
        /// Appends contents to the container.
        /// </summary>
        /// <param name="content">The content to append</param>
        public void Append(XmlReportTag content)
        {
            contents.Add(content);
        }
    }
}
