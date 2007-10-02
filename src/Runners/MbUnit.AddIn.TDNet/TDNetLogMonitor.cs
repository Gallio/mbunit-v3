// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.AddIn.TDNet.Properties;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Core.Model;
using TestDriven.Framework;
using TDF = TestDriven.Framework;
using TestResult = TestDriven.Framework.TestResult;
using TestState = TestDriven.Framework.TestState;

namespace MbUnit.AddIn.TDNet
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

            reportMonitor.StepFinished += HandleStepFinished;
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            reportMonitor.StepFinished -= HandleStepFinished;
        }

        private void HandleStepFinished(object sender, ReportStepEventArgs e)
        {
            // Ignore tests that aren't test cases.
            TestData testData = e.Report.TestModel.Tests[e.TestRun.TestId];
            if (!testData.IsTestCase)
                return;

            // A TestResult with State == TestState.Passed won't be displayed in the
            // output window (TD.NET just diplays "[TestName] passed" in the status bar.
            // Since that can be harder to notice, and also is lost when the following
            // test finishes, we print a message in the output window so the user can 
            // progressively see if the tests are passing or failing.
            if (e.StepRun.Result.Outcome == TestOutcome.Passed)
            {
                testListener.WriteLine(String.Format(Resources.TDNetLogMonitor_TestCasePassed, 
                    e.StepRun.StepFullName), Category.Info);
            }

            // Inform TD.NET what happened 
            TestResult result = new TestResult();
            result.Name = e.StepRun.StepFullName;
            result.TimeSpan = TimeSpan.FromSeconds(e.StepRun.Result.Duration);
            try
            {
                // This could crash in older versions of TD.NET with a MissingFieldException
                // For some reason the exception its always thrown regardless of the catch
                // block so we have to comment it out for now
                //result.TestRunner = "MbUnit Gallio";
            }
            catch (MissingFieldException)
            {
            }

            // It's important to set the stack trace here so the user can double-click in the
            // output window to go the faulting line
            ExecutionLogStream failureStream = e.StepRun.ExecutionLog.GetStream(LogStreamNames.Failures);
            if (failureStream != null)
                result.StackTrace = failureStream.ToString();

            ExecutionLogStream warningStream = e.StepRun.ExecutionLog.GetStream(LogStreamNames.Warnings);
            if (warningStream != null)
                result.Message = String.Format(Resources.TDNetLogMonitor_Warnings, warningStream);

            // TD.NET will automatically count the number of passed, ignored and failed tests
            // provided we call the TestFinished method with the right State
            result.State = GetTestState(e.StepRun.Result.Outcome);

            testListener.TestFinished(result);
        }

        private static TestState GetTestState(TestOutcome outcome)
        {
            switch (outcome)
            {
                case TestOutcome.Failed:
                    return TestState.Failed;
                case TestOutcome.Inconclusive:
                    return TestState.Ignored;
                default:
                    return TestState.Passed;
            }
        }
    }
}
