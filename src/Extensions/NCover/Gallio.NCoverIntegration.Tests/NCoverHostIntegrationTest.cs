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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Runner;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.NCoverIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(NCoverHost))]
    [TestsOn(typeof(NCoverHostFactory))]
    public class NCoverHostIntegrationTest
    {
        private const string TestCoverageXmlFileName = "Coverage.xml";

        [Test]
        [Row("NCover", 0)]
        [Row("NCover2", 2)]
        [Row("NCover3", 3)]
        public void GeneratesNCoverCoverageLogInWorkingDirectory(string factoryName, int majorVersion)
        {
            string tempPath = Path.GetTempPath();
            string coverageFilePath = Path.Combine(tempPath, TestCoverageXmlFileName);

            if (File.Exists(coverageFilePath))
                File.Delete(coverageFilePath);

            Type simpleTestType = typeof(SimpleTest);

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = new TestLogStreamLogger(TestLog.Default);
            launcher.TestPackageConfig.AssemblyFiles.Add(AssemblyUtils.GetAssemblyLocalPath(simpleTestType.Assembly));
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = tempPath;
            launcher.TestRunnerFactoryName = factoryName;
            launcher.TestExecutionOptions.Filter = new TypeFilter<ITest>(new EqualityFilter<string>(simpleTestType.FullName), false);

            if (majorVersion != 0 && !NCoverTool.IsNCoverVersionInstalled(majorVersion))
            {
                var ex = Assert.Throws<RunnerException>(() => launcher.Run());
                Assert.Contains(ex.ToString(), "NCover v" + majorVersion + " does not appear to be installed.");
            }
            else
            {
                TestLauncherResult result = launcher.Run();

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
            }
        }
    }
}
