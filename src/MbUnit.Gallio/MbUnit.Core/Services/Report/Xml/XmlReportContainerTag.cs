using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
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
            set { contents = new List<XmlReportTag>(value); }
        }

        /// <summary>
        /// Adds a content tag to the container.
        /// </summary>
        /// <param name="tag">The tag to add</param>
        public void AddContent(XmlReportTag tag)
        {
            contents.Add(tag);
        }

        /// <summary>
        /// Initializes the contents list to an empty list.
        /// </summary>
        protected void Initialize()
        {
            contents = new List<XmlReportTag>();
        }
    }
}
