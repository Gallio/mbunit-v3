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
using Gallio.Model.Execution;
using Gallio.TDNetRunner.Properties;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;
using TestDriven.Framework;
using ITestListener=TestDriven.Framework.ITestListener;
using TDF = TestDriven.Framework;
using TestResult=TestDriven.Framework.TestResult;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// An <see cref="ITestRunnerMonitor" /> implementation that informs TD.NET (and therefore
    /// the user) in real-time what's going with the tests.
    /// </summary>
    internal class TDNetLogMonitor : BaseTestRunnerMonitor
    {
        private readonly ITestListener testListener;
        private readonly ReportMonitor reportMonitor;

        public TDNetLogMonitor(ITestListener testListener, ReportMonitor reportMonitor)
        {
            if (testListener == null)
                throw new ArgumentNullException(@"testListener");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.testListener = testListener;
            this.reportMonitor = reportMonitor;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            reportMonitor.TestStepFinished += HandleTestStepFinished;
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            reportMonitor.TestStepFinished -= HandleTestStepFinished;
        }

        private void HandleTestStepFinished(object sender, TestStepRunEventArgs e)
        {
            // Ignore tests that aren't test cases.
            // Also ignore nested test steps.
            TestData testData = e.Test;
            if (!testData.IsTestCase || e.TestStepRun.Step.ParentId != null)
                return;

            // A TestResult with State == TestState.Passed won't be displayed in the
            // output window (TD.NET just diplays "[TestName] passed" in the status bar.
            // Since that can be harder to notice, and also is lost when the following
            // test finishes, we print a message in the output window so the user can 
            // progressively see if the tests are passing or failing.
            if (e.TestStepRun.Result.Outcome.Status == TestStatus.Passed)
            {
                testListener.WriteLine(String.Format(Resources.TDNetLogMonitor_TestCasePassed, 
                    e.TestStepRun.Step.FullName), Category.Info);
            }

            // Inform TD.NET what happened 
            TestResult result = new TestResult();
            result.Name = e.TestStepRun.Step.FullName;
            result.TimeSpan = TimeSpan.FromSeconds(e.TestStepRun.Result.Duration);
            // result.TestRunner = "Gallio"; // note: can crash in older versions of TD.Net with MissingFieldException

            // It's important to set the stack trace here so the user can double-click in the
            // output window to go the faulting line
            ExecutionLogStream failureStream = e.TestStepRun.ExecutionLog.GetStream(LogStreamNames.Failures);
            if (failureStream != null)
                result.StackTrace = failureStream.ToString();

            ExecutionLogStream warningStream = e.TestStepRun.ExecutionLog.GetStream(LogStreamNames.Warnings);
            if (warningStream != null)
                result.Message = String.Format(Resources.TDNetLogMonitor_Warnings, warningStream);

            // TD.NET will automatically count the number of passed, ignored and failed tests
            // provided we call the TestFinished method with the right State
            result.State = GetTestState(e.TestStepRun.Result.Outcome);

            testListener.TestFinished(result);
        }

        private static TestState GetTestState(TestOutcome outcome)
        {
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return TestState.Passed;

                case TestStatus.Skipped:
                    return TestState.Ignored;

                default:
                    return TestState.Failed;
            }
        }
    }
}
