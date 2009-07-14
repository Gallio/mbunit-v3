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
using System.Diagnostics;
using System.IO;
using System.Xml;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Common.Markup;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.MSTestAdapter.Properties;
using Gallio.MSTestAdapter.Wrapper;
using Gallio.Common.Caching;
using Gallio.Runtime.ProgressMonitoring;
using TestStep=Gallio.Model.Tree.TestStep;

namespace Gallio.MSTestAdapter.Model
{
    internal class MSTestController : TestController
    {
        private readonly Version frameworkVersion;
        private readonly IDiskCache diskCache;

        internal MSTestController(Version frameworkVersion, IDiskCache diskCache)
        {
            if (frameworkVersion == null)
                throw new ArgumentNullException("frameworkVersion");
            if (diskCache == null)
                throw new ArgumentNullException("diskCache");

            this.frameworkVersion = frameworkVersion;
            this.diskCache = diskCache;
        }

        public static MSTestController CreateController(Version frameworkVersion)
        {
            return new MSTestController(frameworkVersion, new TemporaryDiskCache());
        }

        /// <inheritdoc />
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.MSTestController_RunningMSTestTests, rootTestCommand.TestCount))
            {
                if (options.SkipTestExecution)
                {
                    return SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    return RunTest(rootTestCommand, parentTestStep, progressMonitor);
                }
            }
        }

        private TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            // The first test should be an assembly test
            MSTestAssembly assemblyTest = testCommand.Test as MSTestAssembly;
            TestOutcome outcome;
            TestResult result;
            if (assemblyTest != null)
            {
                ITestContext assemblyContext = testCommand.StartPrimaryChildStep(parentTestStep);
                try
                {
                    MSTestRunner runner = MSTestRunner.GetRunnerForFrameworkVersion(frameworkVersion, diskCache);

                    outcome = runner.RunSession(assemblyContext, assemblyTest,
                        testCommand, parentTestStep, progressMonitor);
                }
                catch (Exception ex)
                {
                    assemblyContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                    outcome = TestOutcome.Error;
                }

                result = assemblyContext.FinishStep(outcome, null);
            }
            else
            {
                result = new TestResult(TestOutcome.Skipped);
            }

            progressMonitor.Worked(1);
            return result;
        }
    }
}
