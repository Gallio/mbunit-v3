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
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A test controller that recursively traverses a hierarchy of test commands and
    /// delegates to another test controller based on a <see cref="TestControllerProvider" />.
    /// </summary>
    public class DelegatingTestController : TestController
    {
        private readonly TestControllerProvider testControllerProvider;

        /// <summary>
        /// Creates a recursive test controller.
        /// </summary>
        /// <param name="testControllerProvider">The test controller provider which provides
        /// the test controller to which the execution of a subtree of tests should be delegated.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testControllerProvider"/>
        /// is null.</exception>
        public DelegatingTestController(TestControllerProvider testControllerProvider)
        {
            if (testControllerProvider == null)
                throw new ArgumentNullException("testControllerProvider");

            this.testControllerProvider = testControllerProvider;
        }

        /// <inheritdoc />
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount))
            {
                return RunTest(rootTestCommand, parentTestStep, options, progressMonitor);
            }
        }

        private TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (TestController testController = testControllerProvider(testCommand.Test))
            {
                if (testController != null)
                    return RunWithController(testController, testCommand, parentTestStep, options, progressMonitor);
            }

            return RunWithoutController(testCommand, parentTestStep, options, progressMonitor);
        }

        private static TestResult RunWithController(TestController testController, ITestCommand testCommand,
            TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            try
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(testCommand.TestCount))
                {
                    return testController.Run(testCommand, parentTestStep, options, subProgressMonitor);
                }
            }
            catch (Exception ex)
            {
                ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
                context.LogWriter.Failures.WriteException(ex, "Fatal Exception in test controller");
                return context.FinishStep(TestOutcome.Error, null);
            }
        }

        private TestResult RunWithoutController(ITestCommand testCommand, TestStep parentTestStep,
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

                TestResult childResult = RunTest(monitor, testContext.TestStep, options, progressMonitor);
                outcome = outcome.CombineWith(childResult.Outcome).Generalize();
            }

            if (progressMonitor.IsCanceled)
                outcome = TestOutcome.Canceled;

            TestResult result = testContext.FinishStep(outcome, null);

            progressMonitor.Worked(1);
            return result;
        }
    }
}
