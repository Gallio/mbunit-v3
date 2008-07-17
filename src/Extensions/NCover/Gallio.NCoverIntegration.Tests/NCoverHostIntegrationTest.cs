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
using System.IO;
using Gallio.Framework;
using Gallio.Framework.Utilities;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Reflection;
using Gallio.Runner;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.NCoverIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(NCoverHost))]
    [TestsOn(typeof(NCoverHostFactory))]
#if NCOVER2
    [TestsOn(typeof(NCoverTool))]
    [Ignore(@"Broken due to a fatal exception in NCover:
-- Top-level exception (System.InvalidOperationException)
Could not merge modules, there is a sequence-point mismatch between multiple loads of method [.cctor..cctor() : void [static]]
-- Stack Trace
at NCover.Framework.CoverageData._MergeMethod(MethodData leftSide, MethodData rightSide)")]
#else
    [TestsOn(typeof(NCoverProcessTask))]
#endif
    public class NCoverHostIntegrationTest
    {
#if NCOVER2
        private const string NCoverTestRunnerFactoryName = "NCover2";
#else
        private const string NCoverTestRunnerFactoryName = "NCover";
#endif
        private const string TestCoverageXmlFileName = "Coverage.xml";

        [Test]
        public void GeneratesNCoverCoverageLogInWorkingDirectory()
        {
            string tempPath = Path.GetTempPath();
            string coverageFilePath = Path.Combine(tempPath, TestCoverageXmlFileName);

            if (File.Exists(coverageFilePath))
                File.Delete(coverageFilePath);

            Type simpleTestType = typeof(SimpleTest);

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = new LogStreamLogger(Log.Default);
            launcher.TestPackageConfig.AssemblyFiles.Add(AssemblyUtils.GetAssemblyLocalPath(simpleTestType.Assembly));
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = tempPath;
            launcher.TestRunnerFactoryName = NCoverTestRunnerFactoryName;
            launcher.TestExecutionOptions.Filter = new TypeFilter<ITest>(new EqualityFilter<string>(simpleTestType.FullName), false);

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

            Assert.Contains(coverageXml, simpleTestType.Name);
        }
    }
}
