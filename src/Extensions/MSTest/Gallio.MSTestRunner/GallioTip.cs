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
using System.Collections;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.MSTestRunner.Runtime;
using Gallio.Runner;
using Gallio.Runner.Extensions;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.VisualStudio.TestTools.Common;
using TestResult=Microsoft.VisualStudio.TestTools.Common.TestResult;

namespace Gallio.MSTestRunner
{
    /// <summary>
    /// Integrates the Gallio test model as an extension for MSTest.
    /// This enables MSTest to run Gallio tests and to display them in the IDE.
    /// </summary>
    internal class GallioTip : Tip
    {
        private readonly ITmi tmi;

        public GallioTip(ITmi tmi)
        {
            if (tmi == null)
                throw new ArgumentNullException("tmi");

            this.tmi = tmi;
        }

        public override ICollection Load(string location, ProjectData projectData, IWarningHandler warningHandler)
        {
            ITestRunnerManager runnerManager = RuntimeProvider.GetRuntime().Resolve<ITestRunnerManager>();
            ITestRunner runner = runnerManager.CreateTestRunner(StandardTestRunnerFactoryNames.Local);

            ArrayList tests = new ArrayList();
            try
            {
                runner.RegisterExtension(new LogExtension());

                WarningLogger logger = new WarningLogger(warningHandler);
                TestRunnerOptions testRunnerOptions = new TestRunnerOptions();
                runner.Initialize(testRunnerOptions, logger, NullProgressMonitor.CreateInstance());

                TestPackageConfig testPackageConfig = new TestPackageConfig();
                testPackageConfig.AssemblyFiles.Add(location);
                runner.Load(testPackageConfig, NullProgressMonitor.CreateInstance());

                TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
                runner.Explore(testExplorationOptions, NullProgressMonitor.CreateInstance());

                TestModelData model = runner.Report.TestModel;
                foreach (TestData test in model.AllTests)
                    if (test.IsTestCase)
                        tests.Add(new GallioTestElement(test, location));
            }
            finally
            {
                runner.Dispose(NullProgressMonitor.CreateInstance());
            }

            return tests;
        }

        public override void Save(ITestElement[] tests, string location, ProjectData projectData)
        {
            throw new NotSupportedException();
        }

        public override TestResult MergeResults(TestResult inMemory, TestResultMessage fromTheWire)
        {
            // Use the base code for merging results.
            TestResult testResult = base.MergeResults(inMemory, fromTheWire);

            // If the base code did not handle our result type, then do extra work.
            GallioTestResult gallioTestResult = testResult as GallioTestResult;
            if (gallioTestResult == null)
            {
                gallioTestResult = new GallioTestResult(testResult);

                GallioTestResult source = inMemory as GallioTestResult;
                if (source != null)
                    gallioTestResult.MergeFrom(source);
            }

            return gallioTestResult;
        }

        public override TestType TestType
        {
            get { return GallioTestTypes.Test; }
        }
    }
}
