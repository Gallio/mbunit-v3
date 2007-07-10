using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An Xml-serializable block of preformatted text to include in a report.
    /// </summary>
    [XmlType]
    [Serializable]
    public sealed class XmlReportTextTag : XmlReportTag
    {
        private string text = "";

        /// <summary>
        /// Gets or sets the text within the tag, not null.
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                text = value;
            }
        }
    }
}
