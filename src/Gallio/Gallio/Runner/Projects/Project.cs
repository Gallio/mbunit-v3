// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Model;
using Gallio.Utilities;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// Gallio project container.
    /// </summary>
    [Serializable]
    [XmlRoot("project", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class Project
    {
        private TestPackageConfig testPackageConfig;
        private List<FilterInfo> testFilters;

        /// <summary>
        /// Creates an empty project.
        /// </summary>
        public Project()
        {
            testPackageConfig = new TestPackageConfig();
            testFilters = new List<FilterInfo>();
        }

        /// <summary>
        /// The test package.
        /// </summary>
        [XmlElement("testPackageConfig", IsNullable=false)]
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
            set { testPackageConfig = value; }
        }

        /// <summary>
        /// A list of test filters for the project.
        /// </summary>
        [XmlArray("testFilters", IsNullable=false)]
        [XmlArrayItem("testFilter", typeof(FilterInfo), IsNullable=false)]
        public List<FilterInfo> TestFilters
        {
            get { return testFilters; }
            set { testFilters = value; }
        }
    }
}
