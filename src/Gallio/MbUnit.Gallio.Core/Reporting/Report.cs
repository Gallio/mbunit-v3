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
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Model.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// A report includes a description of the test package, the model objects,
    /// the combined results of all test runs and summary statistics.
    /// </summary>
    [Serializable]
    [XmlRoot("report", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class Report
    {
        private TestPackage package;
        private TemplateModel templateModel;
        private TestModel testModel;
        private PackageRun packageRun;

        /// <summary>
        /// Gets or sets the test package for the report, or null if none.
        /// </summary>
        [XmlElement("package", IsNullable = true)]
        public TestPackage Package
        {
            get { return package; }
            set { package = value; }
        }

        /// <summary>
        /// Gets or sets the template model for the report, or null if none.
        /// </summary>
        [XmlElement("templateModel", IsNullable = true)]
        public TemplateModel TemplateModel
        {
            get { return templateModel; }
            set { templateModel = value; }
        }

        /// <summary>
        /// Gets or sets the test model for the report, or null if none.
        /// </summary>
        [XmlElement("testModel", IsNullable = true)]
        public TestModel TestModel
        {
            get { return testModel; }
            set { testModel = value; }
        }

        /// <summary>
        /// Gets or sets the package run information included in the report, or null if none.
        /// </summary>
        [XmlElement("packageRun", IsNullable = true)]
        public PackageRun PackageRun
        {
            get { return packageRun; }
            set { packageRun = value; }
        }
    }
}
