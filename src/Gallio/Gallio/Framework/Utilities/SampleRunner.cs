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
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Common.Markup;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Model.Filters;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;

namespace Gallio.Framework.Utilities
{
    /// <summary>
    /// Runs sample test cases within an embedded copy of the test runner and provides
    /// access to the resulting test report.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Logs debug output from the embedded test runner while the sample tests run.
    /// </para>
    /// <para>
    /// This utility class is intended to help write integration tests for test framework features.
    /// </para>
    /// <para>
    /// This class is NOT intended to be used for instructional purposes as a template for
    /// creating a new test runner.  Do NOT copy this code and try to make a new test runner
    /// out of it.  Instead, please examine the code for the Gallio Echo test runner and pay
    /// attention to how it uses the <see cref="TestLauncher" />.
    /// </para>
    /// </remarks>
    public class SampleRunner
    {
        private readonly TestPackage testPackage;
        private readonly List<Filter<ITestDescriptor>> filters;
        private readonly TestRunnerOptions testRunnerOptions;
        private TestLauncherResult result;

        /// <summary>
        /// Creates a sample runner.
        /// </summary>
        public SampleRunner()
        {
            testPackage = new TestPackage();
            filters = new List<Filter<ITestDescriptor>>();
            TestRunnerFactoryName = StandardTestRunnerFactoryNames.Local;
            testRunnerOptions = new TestRunnerOptions();
        }

        /// <summary>
        /// Gets the test report that was produced by the test run.
        /// </summary>
        public Report Report
        {
            get { return result.Report; }
        }

        /// <summary>
        /// Gets the result of the test run.
        /// </summary>
        public TestLauncherResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Gets the test package for the test run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned object may be modified prior to running the tests to configure
        /// various parameters of the test run.
        /// </para>
        /// </remarks>
        public TestPackage TestPackage
        {
            get { return testPackage; }
        }

        /// <summary>
        /// Gets or sets the name of the test runner factory to use.
        /// </summary>
        /// <value>The test runner factory name, defaults to <see cref="StandardTestRunnerFactoryNames.Local" /></value>
        public string TestRunnerFactoryName { get; set; }

        /// <summary>
        /// Gets the test runner options.
        /// </summary>
        public TestRunnerOptions TestRunnerOptions
        {
            get { return testRunnerOptions; }
        }

        /// <summary>
        /// Adds a test file to the package configuration, if not already added.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is null.</exception>
        public void AddFile(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            testPackage.AddFile(file);
        }

        /// <summary>
        /// Adds a test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            testPackage.AddFile(new FileInfo(assembly.Location));
        }

        /// <summary>
        /// Adds a test fixture type to the list of filters, and automatically adds its containing
        /// test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="fixtureType">The test fixture type to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fixtureType"/> is null.</exception>
        public void AddFixture(Type fixtureType)
        {
            AddAssembly(fixtureType.Assembly);
            AddFilter(new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(fixtureType.AssemblyQualifiedName), false));
        }

        /// <summary>
        /// Adds a test method to the list of filters, and automatically adds its containing
        /// test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="fixtureType">The test fixture type.</param>
        /// <param name="methodName">The test method name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fixtureType"/> or <paramref name="methodName"/> is null.</exception>
        public void AddMethod(Type fixtureType, string methodName)
        {
            AddAssembly(fixtureType.Assembly);
            AddFilter(new AndFilter<ITestDescriptor>(new Filter<ITestDescriptor>[]
            {
                new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(fixtureType.AssemblyQualifiedName), false),
                new MemberFilter<ITestDescriptor>(new EqualityFilter<string>(methodName))
            }));
        }

        /// <summary>
        /// Adds a test filter to the combined list of filters that select which tests to include in the run,
        /// if not already added.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
        public void AddFilter(Filter<ITestDescriptor> filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            if (!filters.Contains(filter))
                filters.Add(filter);
        }

        /// <summary>
        /// Gets information about the test with the given code reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="codeReference">The code reference of the test.</param>
        /// <returns>The test data, or null if not found.</returns>
        public TestData GetTestData(CodeReference codeReference)
        {
            return GetTestData(test => test.CodeReference == codeReference);
        }

        /// <summary>
        /// Gets information about the test that matches a predicate.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The test data, or null if not found.</returns>
        public TestData GetTestData(Predicate<TestData> predicate)
        {
            foreach (TestData data in Report.TestModel.AllTests)
            {
                if (predicate(data))
                    return data;
            }

            return null;
        }

        /// <summary>
        /// Gets all test step runs with the given code reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="codeReference">The code reference of the test.</param>
        /// <returns>The enumeration of test step runs, or null if not found.</returns>
        public IEnumerable<TestStepRun> GetTestStepRuns(CodeReference codeReference)
        {
            return GetTestStepRuns(run => run.Step.CodeReference == codeReference);
        }

        /// <summary>
        /// Gets all test step runs that match a predicate.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The enumeration of test step runs, or null if not found.</returns>
        public IEnumerable<TestStepRun> GetTestStepRuns(Predicate<TestStepRun> predicate)
        {
            foreach (TestStepRun run in Report.TestPackageRun.AllTestStepRuns)
                if (predicate(run))
                    yield return run;
        }

        /// <summary>
        /// Gets all test step runs with the code reference infered from the method of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="type">The searched type.</param>
        /// <param name="methodName">The name of the searched method.</param>
        /// <returns>The enumeration of test step runs, or null if not found.</returns>
        public IEnumerable<TestStepRun> GetTestStepRuns(Type type, string methodName)
        {
            var codeReference = CodeReference.CreateFromMember(type.GetMethod(methodName));
            return GetTestStepRuns(codeReference);
        }

        /// <summary>
        /// Gets the primary test step run of a test with the given code reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are multiple primary steps, returns the first one found.
        /// </para>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="codeReference">The code reference of the test.</param>
        /// <returns>The first test step run, or null if not found.</returns>
        public TestStepRun GetPrimaryTestStepRun(CodeReference codeReference)
        {
            foreach (TestStepRun run in GetTestStepRuns(codeReference))
                if (run.Step.IsPrimary)
                    return run;

            return null;
        }

        /// <summary>
        /// Gets the primary test step run that match a predicate.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are multiple primary steps, returns the first one found.
        /// </para>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The first test step run, or null if not found.</returns>
        public TestStepRun GetPrimaryTestStepRun(Predicate<TestStepRun> predicate)
        {
            foreach (TestStepRun run in GetTestStepRuns(predicate))
                if (run.Step.IsPrimary)
                    return run;

            return null;
        }

        /// <summary>
        /// Gets the primary test step run of a test with the code reference infered from the method of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are multiple primary steps, returns the first one found.
        /// </para>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="type">The searched type.</param>
        /// <param name="methodName">The name of the searched method.</param>
        /// <returns>The first test step run, or null if not found.</returns>
        public TestStepRun GetPrimaryTestStepRun(Type type, string methodName)
        {
            var codeReference = CodeReference.CreateFromMember(type.GetMethod(methodName));
            return GetPrimaryTestStepRun(codeReference);
        }

        /// <summary>
        /// Gets all test step runs that represent test cases within a test with
        /// the specified code reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can only be called after the tests have run.
        /// </para>
        /// </remarks>
        /// <param name="codeReference">The code reference of the test.</param>
        /// <returns>The first test step run, or null if not found.</returns>
        public IList<TestStepRun> GetTestCaseRunsWithin(CodeReference codeReference)
        {
            List<TestStepRun> runs = new List<TestStepRun>();
            foreach (TestStepRun containerRun in Report.TestPackageRun.AllTestStepRuns)
            {
                if (containerRun.Step.IsPrimary && containerRun.Step.CodeReference == codeReference)
                {
                    foreach (TestStepRun run in containerRun.AllTestStepRuns)
                    {
                        if (run.Step.IsTestCase)
                            runs.Add(run);
                    }
                }
            }

            return runs;
        }

        /// <summary>
        /// Explores the tests but does not run them.
        /// </summary>
        public void Explore()
        {
            Launch(true);
        }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        public void Run()
        {
            Launch(false);
        }

        private void Launch(bool doNoRun)
        {
            MarkupStreamWriter logStreamWriter = TestLog.Default;

            var launcher = new TestLauncher();
            launcher.TestProject.TestPackage = testPackage;
            launcher.Logger = new MarkupStreamLogger(logStreamWriter);
            launcher.TestExecutionOptions.FilterSet = new FilterSet<ITestDescriptor>(new OrFilter<ITestDescriptor>(filters));
            launcher.TestProject.TestRunnerFactoryName = TestRunnerFactoryName;

            string reportDirectory = SpecialPathPolicy.For<SampleRunner>().GetTempDirectory().FullName;
            launcher.TestProject.ReportDirectory = reportDirectory;
            launcher.TestProject.ReportNameFormat = "SampleRunnerReport";
            launcher.ReportFormatterOptions.AddProperty(@"SaveAttachmentContents", @"false");
            launcher.AddReportFormat(@"Text");
            // Disabled because the Xml can get really big and causes problems if the sample runner is used frequently.
            //launcher.AddReportFormat("Xml");

            launcher.DoNotRun = doNoRun;

            GenericCollectionUtils.ForEach(testRunnerOptions.Properties, x => launcher.TestRunnerOptions.AddProperty(x.Key, x.Value));

            using (logStreamWriter.BeginSection("Log Output"))
                result = launcher.Run();

            using (logStreamWriter.BeginSection("Text Report"))
            {
                foreach (string reportPath in result.ReportDocumentPaths)
                {
                    string extension = Path.GetExtension(reportPath);
                    if (extension == ".txt")
                    {
                        logStreamWriter.WriteLine(File.ReadAllText(reportPath));
                    }
                    else if (extension == ".xml")
                    {
                        logStreamWriter.Container.AttachXml(null, File.ReadAllText(reportPath));
                    }

                    File.Delete(reportPath);
                }
            }
        }
    }
}