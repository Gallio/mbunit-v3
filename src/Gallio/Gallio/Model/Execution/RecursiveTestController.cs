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
using Gallio.Runtime.ProgressMonitoring;
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
        protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                return RunNonMasterTest(rootTestCommand, parentTestStep, options, progressMonitor);
            }
        }

        private static TestOutcome RunTest(ITestCommand testCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            Func<ITestController> factory = testCommand.Test.TestControllerFactory;

            if (factory != null)
            {
                // Delegate to the associated controller, if present.
                using (ITestController controller = factory())
                {
                    try
                    {
                        return controller.RunTests(testCommand, parentTestStep,
                            options, progressMonitor.CreateSubProgressMonitor(testCommand.TestCount));
                    }
                    catch (Exception ex)
                    {
                        ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
                        TestLogWriterUtils.WriteException(context.LogWriter, LogStreamNames.Failures, ex, "Fatal Exception in Test Controller");
                        context.FinishStep(TestOutcome.Error, null);
                        return TestOutcome.Error;
                    }
                }
            }
            else
            {
                return RunNonMasterTest(testCommand, parentTestStep, options, progressMonitor);
            }
        }

        private static TestOutcome RunNonMasterTest(ITestCommand testCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            // Enter the scope of the test and recurse until we find a controller.
            progressMonitor.SetStatus(testCommand.Test.FullName);

            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            TestOutcome outcome = TestOutcome.Passed;

            foreach (ITestCommand monitor in testCommand.Children)
            {
                if (progressMonitor.IsCanceled)
                    break;

                outcome = outcome.CombineWith(RunTest(monitor, testContext.TestStep, options, progressMonitor)).Generalize();
            }

            if (progressMonitor.IsCanceled)
                outcome = TestOutcome.Canceled;

            testContext.FinishStep(outcome, null);

            progressMonitor.Worked(1);
            return outcome;
        }
    }
}
