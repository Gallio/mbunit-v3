// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Validation;
using Gallio.Model.Schema;

namespace Gallio.Runner.Projects.Schema
{
    /// <summary>
    /// Describes a test project in a portable manner for serialization.
    /// </summary>
    [Serializable]
    [XmlRoot("testProject", Namespace = SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class TestProjectData : IValidatable
    {
        private TestPackageData testPackage;
        private readonly List<FilterInfo> testFilters;
        private readonly List<string> testRunnerExtensions;
        private string reportNameFormat;
        private string reportDirectory;

        /// <summary>
        /// Creates an empty test project.
        /// </summary>
        public TestProjectData()
        {
            testPackage = new TestPackageData();
            testFilters = new List<FilterInfo>();
            testRunnerExtensions = new List<string>();
            reportNameFormat = TestProject.DefaultReportNameFormat;
            reportDirectory = TestProject.DefaultReportDirectoryRelativePath;
        }

        /// <summary>
        /// Copies the contents of a test project.
        /// </summary>
        /// <param name="source">The source test project.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public TestProjectData(TestProject source)
            : this()
        {
            if (source == null)
                throw new ArgumentNullException("source");

            testPackage = new TestPackageData(source.TestPackage);
            GenericCollectionUtils.ConvertAndAddAll(source.TestFilters, testFilters, x => x.Copy());
            testRunnerExtensions.AddRange(source.TestRunnerExtensionSpecifications);
            reportNameFormat = source.ReportNameFormat;
            reportDirectory = source.ReportDirectory;
        }

        /// <summary>
        /// Gets or sets the test package.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlElement("testPackage", IsNullable = false)]
        public TestPackageData TestPackage
        {
            get { return testPackage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                testPackage = value;
            }
        }

        /// <summary>
        /// Gets a mutable list of named test filters for the project.
        /// </summary>
        [XmlArray("testFilters", IsNullable = false)]
        [XmlArrayItem("testFilter", typeof(FilterInfo), IsNullable = false)]
        public List<FilterInfo> TestFilters
        {
            get { return testFilters; }
        }

        /// <summary>
        /// Gets a mutable list of test runner extensions used by the project.
        /// </summary>
        [XmlArray("extensionSpecifications", IsNullable = false)]
        [XmlArrayItem("extensionSpecification", typeof(string), IsNullable = false)]
        public List<string> TestRunnerExtensions
        {
            get { return testRunnerExtensions; }
        }

        /// <summary>
        /// Gets or sets the folder to save generated reports to. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Relative to project location, if not absolute.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlElement("reportDirectory")]
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                reportDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the format for the filename of generated reports.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlElement("reportNameFormat")]
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                reportNameFormat = value;
            }
        }

        /// <summary>
        /// Initializes a test project with the contents of this structure.
        /// </summary>
        /// <param name="testProject">The test project to populate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testProject"/> is null.</exception>
        public void InitializeTestProject(TestProject testProject)
        {
            if (testProject == null)
                throw new ArgumentNullException("testProject");

            testPackage.InitializeTestPackage(testProject.TestPackage);
            GenericCollectionUtils.ForEach(testFilters, x => testProject.AddTestFilter(x));
            GenericCollectionUtils.ForEach(testRunnerExtensions, x => testProject.AddTestRunnerExtensionSpecification(x));
            testProject.ReportNameFormat = reportNameFormat;
            testProject.ReportDirectory = reportDirectory;
        }

        /// <summary>
        /// Creates a test project with the contents of this structure.
        /// </summary>
        /// <returns>The test project.</returns>
        public TestProject ToTestProject()
        {
            var testProject = new TestProject();
            InitializeTestProject(testProject);
            return testProject;
        }

        /// <summary>
        /// Makes the test project paths relative to the specified base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="basePath"/> is null.</exception>
        public void MakeRelativePaths(string basePath)
        {
            ApplyPathConversion(basePath, FileUtils.MakeRelativePath);

            if (TestPackage != null)
                TestPackage.MakeRelativePaths(basePath);
        }

        /// <summary>
        /// Makes the test project paths absolute given the specified base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="basePath"/> is null.</exception>
        public void MakeAbsolutePaths(string basePath)
        {
            ApplyPathConversion(basePath, FileUtils.MakeAbsolutePath);

            if (TestPackage != null)
                TestPackage.MakeAbsolutePaths(basePath);
        }

        private void ApplyPathConversion(string basePath, Func<string, string, string> converter)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");

            if (reportDirectory != null)
                reportDirectory = converter(reportDirectory, basePath);
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("testPackage", testPackage);
            testPackage.Validate();

            ValidationUtils.ValidateElementsAreNotNull("testFilters", testFilters);
            ValidationUtils.ValidateAll(testFilters);
            ValidationUtils.ValidateElementsAreNotNull("testRunnerExtensions", testRunnerExtensions);
            ValidationUtils.ValidateNotNull("reportNameFormat", reportNameFormat);
            ValidationUtils.ValidateNotNull("reportDirectory", reportDirectory);
        }
    }
}