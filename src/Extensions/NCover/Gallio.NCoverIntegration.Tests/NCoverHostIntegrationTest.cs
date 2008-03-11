// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using Gallio.Hosting;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.NCoverIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(NCoverHost))]
    [TestsOn(typeof(NCoverHostFactory))]
    [TestsOn(typeof(NCoverProcessTask))]
    public class NCoverHostIntegrationTest
    {
        private const string NCoverTestRunnerFactoryName = "NCover";
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
            launcher.TestPackageConfig.AssemblyFiles.Add(Loader.GetAssemblyLocalPath(simpleTestType.Assembly));
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = tempPath;
            launcher.TestRunnerFactoryName = NCoverTestRunnerFactoryName;
            launcher.Filter = new TypeFilter<ITest>(new EqualityFilter<string>(simpleTestType.FullName), false);

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
