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
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test controller that recursively enters the context of each non master-test found.
    /// When a master test is found, instantiates the <see cref="ITestController" /> for it
    /// and hands control over to it for the subtree of tests rooted at the master test.
    /// </summary>
    public class RecursiveTestController : BaseTestController
    {
        /// <inheritdoc />
        protected override void RunTestsInternal(ITestCommand rootTestCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                RunNonMasterTest(rootTestCommand, parentTestStep, options, progressMonitor);
            }
        }

        private static void RunTest(ITestCommand testCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            Func<ITestController> factory = testCommand.Test.TestControllerFactory;

            if (factory != null)
            {
                // Delegate to the associated controller, if present.
                using (ITestController controller = factory())
                {
                    controller.RunTests(testCommand, parentTestStep,
                        options, progressMonitor.CreateSubProgressMonitor(testCommand.TestCount));
                }
            }
            else
            {
                RunNonMasterTest(testCommand, parentTestStep, options, progressMonitor);
            }
        }

        private static void RunNonMasterTest(ITestCommand testCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            // Enter the scope of the test and recurse until we find a controller.
            progressMonitor.SetStatus(testCommand.Test.FullName);

            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            try
            {
                foreach (ITestCommand monitor in testCommand.Children)
                    RunTest(monitor, testContext.TestStep, options, progressMonitor);
            }
            finally
            {
                testContext.FinishStep(TestOutcome.Passed, null);

                progressMonitor.Worked(1);
            }
        }
    }
}
