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
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner
{
    /// <summary>
    /// Gallio project container.
    /// </summary>
    [Serializable, XmlRoot("gallioProject", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class GallioProject
    {
        private TestPackageConfig testPackageConfig;
        private XmlSerializableDictionary<string, string> testFilters;

        /// <summary>
        /// Creates an empty project.
        /// </summary>
        public GallioProject()
        {
            testPackageConfig = new TestPackageConfig();
            testFilters = new XmlSerializableDictionary<string, string>();
        }

        /// <summary>
        /// The test package.
        /// </summary>
        [XmlElement("testPackageConfig")]
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
            set { testPackageConfig = value; }
        }

        /// <summary>
        /// A list of test filters for the project.
        /// </summary>
        [XmlElement("testFilters")]
        public XmlSerializableDictionary<string, string> TestFilters
        {
            get { return testFilters; }
            set { testFilters = value; }
        }
    }
}
