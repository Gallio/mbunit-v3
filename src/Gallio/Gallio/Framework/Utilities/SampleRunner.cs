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
using System.IO;
using System.Reflection;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Runner;
using Gallio.Model.Filters;

namespace Gallio.Framework.Utilities
{
    /// <summary>
    /// <para>
    /// Runs sample test cases within an embedded copy of the test runner and provides
    /// access to the resulting test report.  Logs debug output from the embedded test runner
    /// while the sample tests run.
    /// </para>
    /// <para>
    /// This utility class is intended to help write integration tests for test framework features.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is NOT intended to be used for instructional purposes as a template for
    /// creating a new test runner.  Do NOT copy this code and try to make a new test runner
    /// out of it.  Instead, please examine the code for the Gallio Echo test runner and pay
    /// attention to how it uses the <see cref="TestLauncher" />.
    /// </para>
    /// </remarks>
    public class SampleRunner
    {
        private readonly TestPackageConfig packageConfig;
        private readonly List<Filter<ITest>> filters;
        private TestLauncherResult result;

        /// <summary>
        /// Creates a sample runner.
        /// </summary>
        public SampleRunner()
        {
            packageConfig = new TestPackageConfig();
            filters = new List<Filter<ITest>>();
            TestRunnerFactoryName = StandardTestRunnerFactoryNames.Local;
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
        /// <para>
        /// Gets the package configuration object for the test run.
        /// </para>
        /// <para>
        /// The returned object may be modified prior to running the tests to configure
        /// various parameters of the test run.
        /// </para>
        /// </summary>
        public TestPackageConfig PackageConfig
        {
            get { return packageConfig; }
        }

        /// <summary>
        /// Gets or sets the name of the test runner factory to use.
        /// </summary>
        /// <value>The test runner factory name, defaults to <see cref="StandardTestRunnerFactoryNames.Local" /></value>
        public string TestRunnerFactoryName { get; set; }

        /// <summary>
        /// Adds a test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="assembly">The assembly to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            string assemblyFile = assembly.Location;
            if (!packageConfig.AssemblyFiles.Contains(assemblyFile))
                packageConfig.AssemblyFiles.Add(assemblyFile);
        }

        /// <summary>
        /// Adds a test fixture type to the list of filters, and automatically adds its containing
        /// test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="fixtureType">The test fixture type to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fixtureType"/> is null</exception>
        public void AddFixture(Type fixtureType)
        {
            AddAssembly(fixtureType.Assembly);
            AddFilter(new TypeFilter<ITest>(new EqualityFilter<string>(fixtureType.AssemblyQualifiedName), false));
        }

        /// <summary>
        /// Adds a test method to the list of filters, and automatically adds its containing
        /// test assembly to the package configuration, if not already added.
        /// </summary>
        /// <param name="fixtureType">The test fixture type</param>
        /// <param name="methodName">The test method name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fixtureType"/> or <paramref name="methodName"/> is null</exception>
        public void AddMethod(Type fixtureType, string methodName)
        {
            AddAssembly(fixtureType.Assembly);
            AddFilter(new AndFilter<ITest>(new Filter<ITest>[]
            {
                new TypeFilter<ITest>(new EqualityFilter<string>(fixtureType.AssemblyQualifiedName), false),
                new MemberFilter<ITest>(new EqualityFilter<string>(methodName))
            }));
        }

        /// <summary>
        /// Adds a test filter to the combined list of filters that select which tests to include in the run,
        /// if not already added.
        /// </summary>
        /// <param name="filter">The filter to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null</exception>
        public void AddFilter(Filter<ITest> filter)
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
        /// Can only be called after the tests have run.
        /// </remarks>
        /// <param name="codeReference">The code reference of the test</param>
        /// <returns>The test data, or null if not found</returns>
        public TestData GetTestData(CodeReference codeReference)
        {
            foreach (TestData data in Report.TestModel.AllTests)
            {
                if (data.CodeReference == codeReference)
                    return data;
            }

            return null;
        }

        /// <summary>
        /// Gets all test step runs with the given code reference.
        /// </summary>
        /// <remarks>
        /// Can only be called after the tests have run.
        /// </remarks>
        /// <param name="codeReference">The code reference of the test</param>
        /// <returns>The enumeration of test step runs, or null if not found</returns>
        public IEnumerable<TestStepRun> GetTestStepRuns(CodeReference codeReference)
        {
            foreach (TestStepRun run in Report.TestPackageRun.AllTestStepRuns)
                if (run.Step.CodeReference == codeReference)
                    yield return run;
        }

        /// <summary>
        /// Gets the primary test step run of a test with the given code reference.
        /// If there are multiple primary steps, returns the first one found.
        /// </summary>
        /// <remarks>
        /// Can only be called after the tests have run.
        /// </remarks>
        /// <param name="codeReference">The code reference of the test</param>
        /// <returns>The first test step run, or null if not found</returns>
        public TestStepRun GetPrimaryTestStepRun(CodeReference codeReference)
        {
            foreach (TestStepRun run in GetTestStepRuns(codeReference))
                if (run.Step.IsPrimary)
                    return run;

            return null;
        }

        /// <summary>
        /// Gets all test step runs that represent test cases within a test with
        /// the specified code reference.
        /// </summary>
        /// <remarks>
        /// Can only be called after the tests have run.
        /// </remarks>
        /// <param name="codeReference">The code reference of the test</param>
        /// <returns>The first test step run, or null if not found</returns>
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
            TestLogStreamWriter logStreamWriter = TestLog.Default;

            TestLauncher launcher = new TestLauncher();
            launcher.TestPackageConfig = packageConfig;
            launcher.Logger = new TestLogStreamLogger(logStreamWriter);
            launcher.TestExecutionOptions.Filter = new OrFilter<ITest>(filters.ToArray());
            launcher.TestRunnerFactoryName = TestRunnerFactoryName;

            string reportDirectory = Path.GetTempPath();
            launcher.ReportDirectory = reportDirectory;
            launcher.ReportNameFormat = "SampleRunnerReport";
            launcher.ReportFormatOptions.Add(@"SaveAttachmentContents", @"false");
            launcher.ReportFormats.Add(@"Text");

            launcher.DoNotRun = doNoRun;

            using (logStreamWriter.BeginSection("Log Output"))
                result = launcher.Run();

            using (logStreamWriter.BeginSection("Text Report"))
            {
                foreach (string reportPath in result.ReportDocumentPaths)
                {
                    logStreamWriter.WriteLine(File.ReadAllText(reportPath));
                    File.Delete(reportPath);
                }
            }
        }
    }
}