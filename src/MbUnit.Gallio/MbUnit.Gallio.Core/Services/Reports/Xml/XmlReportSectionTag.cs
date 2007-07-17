// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
