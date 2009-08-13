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
using System.IO;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Framework;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.NCoverIntegration.Tools;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.NCoverIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(NCoverTestIsolationProvider))]
    public class NCoverTestIsolationProviderIntegrationTest
    {
        [Test]
        [Row("NCover", NCoverVersion.V1, null, false, false)]
        [Row("NCover2", NCoverVersion.V2, null, false, false)]
        [Row("NCover3", NCoverVersion.V3, null, false, false)]
        [Row("NCover", NCoverVersion.V1, "DifferentFile.xml", true, true)]
        [Row("NCover2", NCoverVersion.V2, "DifferentFile.xml", true, true)]
        [Row("NCover3", NCoverVersion.V3, "DifferentFile.xml", true, true)]
        public void GeneratesNCoverCoverageWithCorrectOptionsAndMergesIfNecessary(string factoryName, NCoverVersion version,
            string ncoverCoverageFile, bool includeLogArgument, bool runMultipleAssemblies)
        {
            string tempPath = SpecialPathPolicy.For<NCoverTestIsolationProviderIntegrationTest>().GetTempDirectory().FullName;
            string coverageFilePath = Path.Combine(tempPath, ncoverCoverageFile ?? "Coverage.xml");
            string coverageLogFilePath = Path.Combine(tempPath, "CoverageLog.txt");

            string ncoverArguments = includeLogArgument
                ? "//l \"" + coverageLogFilePath + "\"" + (version == NCoverVersion.V1 ? "" : " //ll Normal")
                : null;

            if (File.Exists(coverageFilePath))
                File.Delete(coverageFilePath);

            if (File.Exists(coverageLogFilePath))
                File.Delete(coverageLogFilePath);

            Type firstTestType = typeof(SimpleTest);
            Type secondTestType = typeof(DummyTest);

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = new MarkupStreamLogger(TestLog.Default);

            launcher.TestProject.TestPackage.AddFile(new FileInfo(AssemblyUtils.GetAssemblyLocalPath(firstTestType.Assembly)));
            if (runMultipleAssemblies)
                launcher.TestProject.TestPackage.AddFile(new FileInfo(AssemblyUtils.GetAssemblyLocalPath(secondTestType.Assembly)));

            launcher.TestProject.TestRunnerFactoryName = factoryName;
            launcher.TestExecutionOptions.FilterSet = new FilterSet<ITestDescriptor>(
                new OrFilter<ITestDescriptor>(new[]
                    {
                        new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(firstTestType.FullName), false),
                        new TypeFilter<ITestDescriptor>(new EqualityFilter<string>(secondTestType.FullName), false),
                    }));

            if (ncoverArguments != null)
                launcher.TestRunnerOptions.AddProperty("NCoverArguments", ncoverArguments);
            if (ncoverCoverageFile != null)
                launcher.TestRunnerOptions.AddProperty("NCoverCoverageFile", ncoverCoverageFile);

            TestLauncherResult result;
            using (new CurrentDirectorySwitcher(tempPath))
                result = launcher.Run();

            NCoverTool tool = NCoverTool.GetInstance(version);
            if (! tool.IsInstalled())
            {
                Assert.AreEqual(ResultCode.Failure, result.ResultCode);

                var logEntries = result.Report.LogEntries;
                Assert.AreEqual(1, logEntries.Count, "Expected to find a log entry.");
                Assert.AreEqual(LogSeverity.Error, logEntries[0].Severity);
                Assert.Contains(logEntries[0].Details, tool.Name + " does not appear to be installed.");
            }
            else
            {
                int passed = runMultipleAssemblies ? 2 : 1;
                int failed = 1;

                Assert.AreEqual(passed + failed, result.Statistics.RunCount);
                Assert.AreEqual(passed, result.Statistics.PassedCount);
                Assert.AreEqual(failed, result.Statistics.FailedCount);
                Assert.AreEqual(0, result.Statistics.InconclusiveCount);
                Assert.AreEqual(0, result.Statistics.SkippedCount);

                Assert.IsTrue(File.Exists(coverageFilePath),
                    "The NCover runner should have written its coverage log to '{0}'.", coverageFilePath);

                string coverageXml = File.ReadAllText(coverageFilePath);
                File.Delete(coverageFilePath);

                Assert.Contains(coverageXml, firstTestType.Name,
                    "Expected the coverage log to include information about the test method that we actually ran.\n"
                    + "In NCover v3, there is now a list of excluded 'system assemblies' in NCover.Console.exe.config which "
                    + "specifies MbUnit and Gallio by default.  For this test to run, the file must be edited such that "
                    + "these entries are removed.");

                if (runMultipleAssemblies)
                {
                    Assert.Contains(coverageXml, secondTestType.Name,
                        "Expected the coverage log to include information about the test method that we actually ran.\n"
                        + "In NCover v3, there is now a list of excluded 'system assemblies' in NCover.Console.exe.config which "
                        + "specifies MbUnit and Gallio by default.  For this test to run, the file must be edited such that "
                        + "these entries are removed.");
                }

                if (ncoverArguments != null && ncoverArguments.Contains("/l"))
                {
                    Assert.IsTrue(File.Exists(coverageLogFilePath),
                        "Should have created a coverage log file since the /l argument was specified.");
                    File.Delete(coverageLogFilePath);
                }
            }
        }
    }
}
