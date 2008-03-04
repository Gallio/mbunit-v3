// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Serialization;
using Gallio.Model;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report includes a description of the test package, the model objects,
    /// the combined results of all test runs and summary statistics.
    /// </summary>
    [Serializable]
    [XmlRoot("report", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public sealed class Report
    {
        private TestPackageConfig packageConfig;
        private TestModelData testModelData;
        private PackageRun packageRun;

        /// <summary>
        /// Gets or sets the test package configuration for the report, or null if none.
        /// </summary>
        [XmlElement("packageConfig", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
        public TestPackageConfig PackageConfig
        {
            get { return packageConfig; }
            set { packageConfig = value; }
        }

        /// <summary>
        /// Gets or sets the test model for the report, or null if none.
        /// </summary>
        [XmlElement("testModel", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        /// <summary>
        /// Gets or sets the package run information included in the report, or null if none.
        /// </summary>
        [XmlElement("packageRun", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
        public PackageRun PackageRun
        {
            get { return packageRun; }
            set { packageRun = value; }
        }
    }
}
