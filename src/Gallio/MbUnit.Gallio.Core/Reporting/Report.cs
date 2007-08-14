using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Serialization;
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
        [XmlElement("run", IsNullable = true)]
        public PackageRun PackageRun
        {
            get { return packageRun; }
            set { packageRun = value; }
        }
    }
}
