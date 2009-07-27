// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.ObjectModel;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Runner.Extensions;
using Gallio.Runner.Projects.Schema;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// A test project consists of a test package with a list of test files, a list
    /// of test extensions, and report options.
    /// </summary>
    public class TestProject
    {
        private TestPackage testPackage;

        private string reportNameFormat;
        private bool isReportNameFormatSpecified;
        private string reportDirectory;
        private bool isReportDirectorySpecified;
        private readonly List<FilterInfo> testFilters;
        private readonly List<ITestRunnerExtension> testRunnerExtensions;
        private readonly List<string> testRunnerExtensionSpecifications;
        private string testRunnerFactoryName;
        private bool isTestRunnerFactoryNameSpecified;

        /// <summary>
        /// The default report name format.
        /// </summary>
        /// <value>"test-report-{0}-{1}"</value>
        public static readonly string DefaultReportNameFormat = @"test-report-{0}-{1}";

        /// <summary>
        /// The default report directory relative path.
        /// </summary>
        /// <value>"Reports"</value>
        public static readonly string DefaultReportDirectoryRelativePath = @".\Reports";

        /// <summary>
        /// The default test runner factory name.
        /// </summary>
        /// <value><see cref="StandardTestRunnerFactoryNames.IsolatedProcess" /></value>
        public static readonly string DefaultTestRunnerFactoryName = StandardTestRunnerFactoryNames.IsolatedProcess;

        /// <summary>
        /// The file extension for Gallio project files.
        /// </summary>
        /// <value>".gallio"</value>
        public static readonly string Extension = ".gallio";

        /// <summary>
        /// Creates an empty test project.
        /// </summary>
        public TestProject()
        {
            testPackage = new TestPackage();
            testFilters = new List<FilterInfo>();
            testRunnerExtensions = new List<ITestRunnerExtension>();
            testRunnerExtensionSpecifications = new List<string>();
            reportNameFormat = DefaultReportNameFormat;
            reportDirectory = DefaultReportDirectoryRelativePath;
            testRunnerFactoryName = DefaultTestRunnerFactoryName;
        }

        /// <summary>
        /// Gets or sets the test package.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestPackage TestPackage
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
        /// Gets a read-only list of test filters.
        /// </summary>
        public IList<FilterInfo> TestFilters
        {
            get { return new ReadOnlyCollection<FilterInfo>(testFilters); }
        }

        /// <summary>
        /// Gets a read-only list of test runner extensions.
        /// </summary>
        public IList<ITestRunnerExtension> TestRunnerExtensions
        {
            get { return new ReadOnlyCollection<ITestRunnerExtension>(testRunnerExtensions); }
        }

        /// <summary>
        /// Gets a read-only list of test runner extension specifications.
        /// </summary>
        /// <seealso cref="TestRunnerExtensionUtils.CreateExtensionFromSpecification"/>
        /// for an explanation of the specification syntax.
        public IList<string> TestRunnerExtensionSpecifications
        {
            get { return new ReadOnlyCollection<string>(testRunnerExtensionSpecifications); }
        }

        /// <summary>
        /// Gets or sets the name of a <see cref="ITestRunnerFactory" /> to use for constructing
        /// the <see cref="ITestRunner" /> at test execution time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <see cref="StandardTestRunnerFactoryNames.IsolatedProcess"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string TestRunnerFactoryName
        {
            get { return testRunnerFactoryName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testRunnerFactoryName = value;
                isTestRunnerFactoryNameSpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="TestRunnerFactoryName" /> has been set explicitly.
        /// </summary>
        public bool IsTestRunnerFactoryNameSpecified
        {
            get { return isTestRunnerFactoryNameSpecified; }
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
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                reportDirectory = value;
                isReportDirectorySpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="ReportDirectory" /> has been set explicitly.
        /// </summary>
        public bool IsReportDirectorySpecified
        {
            get { return isReportDirectorySpecified; }
        }

        /// <summary>
        /// The format for the filename of generated reports.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Within the format string, <c>{0}</c> is replaced by the date and <c>{1}</c> by the time.
        /// </para>
        /// <para>
        /// The default value is <c>"test-report-{0}-{1}"</c>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                reportNameFormat = value;
                isReportNameFormatSpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="ReportNameFormat" /> has been set explicitly.
        /// </summary>
        public bool IsReportNameFormatSpecified
        {
            get { return isReportNameFormatSpecified; }
        }

        /// <summary>
        /// Resets <see cref="TestRunnerFactoryName"/> to its default value and sets <see cref="IsTestRunnerFactoryNameSpecified" /> to false.
        /// </summary>
        public void ResetTestRunnerFactoryName()
        {
            testRunnerFactoryName = DefaultTestRunnerFactoryName;
            isTestRunnerFactoryNameSpecified = false;
        }

        /// <summary>
        /// Resets <see cref="ReportDirectory"/> to its default value and sets <see cref="IsReportDirectorySpecified" /> to false.
        /// </summary>
        public void ResetReportDirectory()
        {
            reportDirectory = DefaultReportDirectoryRelativePath;
            isReportDirectorySpecified = false;
        }

        /// <summary>
        /// Resets <see cref="ReportNameFormat"/> to its default value and sets <see cref="IsReportNameFormatSpecified" /> to false.
        /// </summary>
        public void ResetReportNameFormat()
        {
            reportNameFormat = DefaultReportNameFormat;
            isReportNameFormatSpecified = false;
        }

        /// <summary>
        /// Clears the list of test filters.
        /// </summary>
        public void ClearTestFilters()
        {
            testFilters.Clear();
        }

        /// <summary>
        /// Adds a test filter if not already added.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
        public void AddTestFilter(FilterInfo filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            filter.Validate();
            if (! testFilters.Contains(filter))
                testFilters.Add(filter);
        }

        /// <summary>
        /// Removes a test filter.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
        public void RemoveTestFilter(FilterInfo filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            testFilters.Remove(filter);
        }

        /// <summary>
        /// Clears the list of test runner extensions.
        /// </summary>
        public void ClearTestRunnerExtensions()
        {
            testRunnerExtensions.Clear();
        }

        /// <summary>
        /// Adds a test runner extension if not already added.
        /// </summary>
        /// <param name="extension">The test runner extension to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null.</exception>
        public void AddTestRunnerExtension(ITestRunnerExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            if (!testRunnerExtensions.Contains(extension))
                testRunnerExtensions.Add(extension);
        }

        /// <summary>
        /// Removes a test runner extension.
        /// </summary>
        /// <param name="extension">The test runner extension to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null.</exception>
        public void RemoveTestRunnerExtension(ITestRunnerExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            testRunnerExtensions.Remove(extension);
        }

        /// <summary>
        /// Clears the list of test runner extension specifications.
        /// </summary>
        public void ClearTestRunnerExtensionSpecifications()
        {
            testRunnerExtensionSpecifications.Clear();
        }

        /// <summary>
        /// Adds a test runner extension specification if not already added.
        /// </summary>
        /// <param name="extensionSpecification">The test runner extension specification to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extensionSpecification"/> is null.</exception>
        /// <seealso cref="TestRunnerExtensionUtils.CreateExtensionFromSpecification"/>
        /// for an explanation of the specification syntax.
        public void AddTestRunnerExtensionSpecification(string extensionSpecification)
        {
            if (extensionSpecification == null)
                throw new ArgumentNullException("specification");

            if (!testRunnerExtensionSpecifications.Contains(extensionSpecification))
                testRunnerExtensionSpecifications.Add(extensionSpecification);
        }

        /// <summary>
        /// Removes a test runner extension specification.
        /// </summary>
        /// <param name="extensionSpecification">The test runner extension specification to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extensionSpecification"/> is null.</exception>
        /// <seealso cref="TestRunnerExtensionUtils.CreateExtensionFromSpecification"/>
        /// for an explanation of the specification syntax.
        public void RemoveTestRunnerExtensionSpecification(string extensionSpecification)
        {
            if (extensionSpecification == null)
                throw new ArgumentNullException("specification");

            testRunnerExtensionSpecifications.Remove(extensionSpecification);
        }

        /// <summary>
        /// Creates a copy of the test project.
        /// </summary>
        /// <returns>The new copy.</returns>
        public TestProject Copy()
        {
            TestProject copy = new TestProject()
            {
                testRunnerFactoryName = testRunnerFactoryName,
                isTestRunnerFactoryNameSpecified = isTestRunnerFactoryNameSpecified,
                reportDirectory = reportDirectory,
                isReportDirectorySpecified = isReportDirectorySpecified,
                reportNameFormat = reportNameFormat,
                isReportNameFormatSpecified = isReportNameFormatSpecified,
                testPackage = testPackage.Copy()
            };

            GenericCollectionUtils.ConvertAndAddAll(testFilters, copy.testFilters, x => x.Copy());
            copy.testRunnerExtensions.AddRange(testRunnerExtensions);
            copy.testRunnerExtensionSpecifications.AddRange(testRunnerExtensionSpecifications);
            return copy;
        }

        /// <summary>
        /// Applies the settings of another test project as an overlay on top of this one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Overrides scalar settings (such as <see cref="ReportNameFormat"/>) with those of
        /// the overlay when they are specified (such as when <see cref="IsReportNameFormatSpecified" /> is true).
        /// Merges aggregate settings (such as lists of files).
        /// </para>
        /// </remarks>
        /// <param name="overlay">The test project to overlay on top of this one.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
        public void ApplyOverlay(TestProject overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");

            if (overlay.IsReportNameFormatSpecified)
                ReportNameFormat = overlay.ReportNameFormat;
            if (overlay.IsReportDirectorySpecified)
                ReportDirectory = overlay.ReportDirectory;
            if (overlay.IsTestRunnerFactoryNameSpecified)
                TestRunnerFactoryName = overlay.TestRunnerFactoryName;
            GenericCollectionUtils.ForEach(overlay.TestFilters, x => AddTestFilter(x.Copy()));
            GenericCollectionUtils.ForEach(overlay.TestRunnerExtensions, x => AddTestRunnerExtension(x));
            GenericCollectionUtils.ForEach(overlay.TestRunnerExtensionSpecifications, x => AddTestRunnerExtensionSpecification(x));

            TestPackage.ApplyOverlay(overlay.TestPackage);
        }
    }
}
