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
using MbUnit.Core.Runner.Monitors;
using MbUnit.Framework.Kernel.Results;
using TestDriven.Framework;
using TDF = TestDriven.Framework;
using TestResult = TestDriven.Framework.TestResult;
using TestState = TestDriven.Framework.TestState;

namespace MbUnit.AddIn.TDNet
{
    /// <summary>
    /// An ITestRunnerMonitor implementation that informs TD.NET (and therefore the user)
    /// in real-time what's going with the tests.
    /// </summary>
    internal class TDNetLogMonitor : LogMonitor
    {
        private readonly ITestListener testListener;

        public TDNetLogMonitor(ITestListener testListener)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");

            this.testListener = testListener;
        }

        protected override void OnTestCaseFinished(string testName, string stackTrace, Framework.Kernel.Results.TestResult testResult)
        {
            // A TestResult with State == TestState.Passed won't be displayed in the
            // output window (TD.NET just diplays "[TestName] passed" in the status bar.
            // Since that can be harder to notice, and also is lost when the following
            // test finishes, we print a message in the output window so the user can 
            // progressively see if the tests are passing or failing.
            if (testResult.Outcome == TestOutcome.Passed)
            {
                string message = @"TestCase '{0}' passed [{1} Asserts, Took {2} seconds]";
                string formattedMessage = String.Format(message, testName, testResult.AssertCount, testResult.Duration);
                testListener.WriteLine(formattedMessage, Category.Info);
            }

            // Inform TD.NET what happened 
            TestResult result = new TestResult();
            result.Name = testName;
            // It's important to set the stack trace here so the user can double-click in the
            // output window to go the faulting line
            if (stackTrace != null)
                result.StackTrace = stackTrace;
            // TD.NET will automatically count the number of passed, ignored and failed tests
            // provided we call the TestFinished method with the right State
            result.State = GetTestState(testResult.Outcome);
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
