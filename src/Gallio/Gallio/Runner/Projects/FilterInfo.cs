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
using System.Xml.Serialization;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// Filter record for Gallio project.
    /// </summary>
    [Serializable]
    [XmlRoot("filterInfo", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class FilterInfo
    {
        private string filterName, filter;

        /// <summary>
        /// The name of the filter.
        /// </summary>
        [XmlElement("filterName")]
        public string FilterName
        {
            get { return filterName; }
            set { filterName = value; }
        }

        /// <summary>
        /// A string representation of the filter.
        /// </summary>
        [XmlElement("filter")]
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        /// <summary>
        /// Parameterless constructor for serialisation.
        /// </summary>
        public FilterInfo()
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="filter">The filter itself.</param>
        public FilterInfo(string filterName, string filter)
        {
            this.filterName = filterName;
            this.filter = filter;
        }
    }
}