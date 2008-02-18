// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
        public override void RunTests(IProgressMonitor progressMonitor, ITestCommand rootTestCommand,
            ITestInstance parentTestInstance)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (rootTestCommand == null)
                throw new ArgumentNullException("rootTestCommand");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                RunNonMasterTest(progressMonitor, rootTestCommand, parentTestInstance);
            }
        }

        private static void RunTest(IProgressMonitor progressMonitor, ITestCommand testCommand,
            ITestInstance parentTestInstance)
        {
            Func<ITestController> factory = testCommand.Test.TestControllerFactory;

            if (factory != null)
            {
                // Delegate to the associated controller, if present.
                using (ITestController controller = factory())
                {
                    controller.RunTests(progressMonitor.CreateSubProgressMonitor(testCommand.TestCount),
                        testCommand, parentTestInstance);
                }
            }
            else
            {
                RunNonMasterTest(progressMonitor, testCommand, parentTestInstance);
            }
        }

        private static void RunNonMasterTest(IProgressMonitor progressMonitor, ITestCommand testCommand,
            ITestInstance parentTestInstance)
        {
            // Enter the scope of the test and recurse until we find a controller.
            progressMonitor.SetStatus(String.Format("Run test: {0}.", testCommand.Test.Name));

            ITestContext testContext = testCommand.StartRootStep(parentTestInstance);
            try
            {
                foreach (ITestCommand monitor in testCommand.Children)
                    RunTest(progressMonitor, monitor, testContext.TestStep.TestInstance);
            }
            finally
            {
                testContext.FinishStep(TestStatus.Executed, TestOutcome.Passed, null);

                progressMonitor.Worked(1);
            }
        }
    }
}
