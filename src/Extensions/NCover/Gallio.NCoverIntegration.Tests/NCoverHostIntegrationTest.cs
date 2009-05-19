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
using System.Diagnostics;
using System.IO;
using Gallio.Common.Policies;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.NCoverIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(NCoverHost))]
    [TestsOn(typeof(NCoverHostFactory))]
    public class NCoverHostIntegrationTest
    {
        [Test]
        [Row("NCover", 0, null, false)]
        [Row("NCover2", 2, null, false)]
        [Row("NCover3", 3, null, false)]
        [Row("NCover", 0, "DifferentFile.xml", true)]
        [Row("NCover2", 2, "DifferentFile.xml", true)]
        [Row("NCover3", 3, "DifferentFile.xml", true)]
        public void GeneratesNCoverCoverageAndProcessesHostProperties(string factoryName, int majorVersion,
            string ncoverCoverageFile, bool includeLogArgument)
        {
            if (Process.GetProcessesByName("NCover.Console").Length != 0)
                Assert.Inconclusive("Cannot run this test while another instance of NCover is running.");

            string tempPath = SpecialPathPolicy.For<NCoverHostIntegrationTest>().GetTempDirectory().FullName;
            string coverageFilePath = Path.Combine(tempPath, ncoverCoverageFile ?? "Coverage.xml");
            string coverageLogFilePath = Path.Combine(tempPath, "CoverageLog.txt");

            string ncoverArguments = includeLogArgument
                ? "//l \"" + coverageLogFilePath + "\"" + (majorVersion == 0 ? "" : " //ll Normal")
                : null;

            if (File.Exists(coverageFilePath))
                File.Delete(coverageFilePath);

            if (File.Exists(coverageLogFilePath))
                File.Delete(coverageLogFilePath);

            Type simpleTestType = typeof(SimpleTest);

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = new MarkupStreamLogger(TestLog.Default);
            launcher.TestPackageConfig.AssemblyFiles.Add(AssemblyUtils.GetAssemblyLocalPath(simpleTestType.Assembly));
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = tempPath;
            launcher.TestRunnerFactoryName = factoryName;
            launcher.TestExecutionOptions.FilterSet = new FilterSet<ITest>(new TypeFilter<ITest>(new EqualityFilter<string>(simpleTestType.FullName), false));

            launcher.TestRunnerOptions.Properties.SetValue("NCoverArguments", ncoverArguments);
            launcher.TestRunnerOptions.Properties.SetValue("NCoverCoverageFile", ncoverCoverageFile);

            TestLauncherResult result = launcher.Run();

            if (majorVersion != 0 && !NCoverTool.IsNCoverVersionInstalled(majorVersion))
            {
                Assert.AreEqual(ResultCode.Failure, result.ResultCode);

                var logEntries = result.Report.LogEntries;
                Assert.AreEqual(1, logEntries.Count);
                Assert.AreEqual(LogSeverity.Error, logEntries[0].Severity);
                Assert.Contains(logEntries[0].Details, "NCover v" + majorVersion + " does not appear to be installed.");
            }
            else
            {
                Assert.AreEqual(2, result.Statistics.RunCount);
                Assert.AreEqual(1, result.Statistics.PassedCount);
                Assert.AreEqual(1, result.Statistics.FailedCount);
                Assert.AreEqual(0, result.Statistics.InconclusiveCount);
                Assert.AreEqual(0, result.Statistics.SkippedCount);

                Assert.IsTrue(File.Exists(coverageFilePath),
                    "The NCover runner should have written its coverage log to '{0}'.", coverageFilePath);

                string coverageXml = File.ReadAllText(coverageFilePath);
                File.Delete(coverageFilePath);

                Assert.Contains(coverageXml, simpleTestType.Name,
                    "Expected the coverage log to include information about the test method that we actually ran.\n"
                    + "In NCover v3, there is now a list of excluded 'system assemblies' in NCover.Console.exe.config which "
                    + "specifies MbUnit and Gallio by default.  For this test to run, the file must be edited such that "
                    + "these entries are removed.");

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
