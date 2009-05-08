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
using System.Text;
using Gallio.Model.Logging;
using Gallio.Model.Serialization;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.TDNetRunner.Facade;
using Gallio.TDNetRunner.Properties;
using Gallio.Model;
using TestDriven.Framework;

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// A test runner extension that informs TD.NET (and therefore
    /// the user) in real-time what's going with the tests.
    /// </summary>
    internal class TDNetExtension : LogExtension
    {
        private readonly IFacadeTestListener testListener;

        public TDNetExtension(IFacadeTestListener testListener)
        {
            if (testListener == null)
                throw new ArgumentNullException(@"testListener");

            this.testListener = testListener;
        }

        /// <inheritdoc />
        protected override void LogAnnotation(AnnotationData annotation)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("[{0}] ", annotation.Type.ToString().ToLower());
            message.Append(annotation.Message);

            if (annotation.CodeLocation != CodeLocation.Unknown)
            {
                message.Append("\n\tLocation: ");
                message.Append(annotation.CodeLocation);
            }

            if (annotation.CodeLocation.Line == 0 && annotation.CodeReference != CodeReference.Unknown)
            {
                message.Append("\n\tReference: ");
                message.Append(annotation.CodeReference);
            }

            if (!string.IsNullOrEmpty(annotation.Details))
            {
                message.Append("\n\tDetails: ");
                message.Append(annotation.Details);
            }

            FacadeCategory category = GetCategoryForAnnotation(annotation.Type);
            testListener.WriteLine(message.ToString(), category);
        }

        /// <inheritdoc />
        protected override void LogTestCaseStarted(TestStepStartedEventArgs e)
        {
        }

        /// <inheritdoc />
        protected override void LogTestCaseFinished(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }

        /// <inheritdoc />
        protected override void LogNonTestCaseProblem(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }

        private void LogTest(TestStepFinishedEventArgs e)
        {
            // A TestResult with State == TestState.Passed won't be displayed in the
            // output window (TD.NET just diplays "[TestName] passed" in the status bar.
            // Since that can be harder to notice, and also is lost when the following
            // test finishes, we print a message in the output window so the user can 
            // progressively see if the tests are passing or failing.
            if (e.TestStepRun.Result.Outcome.Status == TestStatus.Passed)
            {
                testListener.WriteLine(String.Format(Resources.TDNetLogMonitor_TestCasePassed,
                    e.TestStepRun.Step.FullName), FacadeCategory.Info);
            }

            // Inform TD.NET what happened 
            FacadeTestResult result = new FacadeTestResult();
            result.Name = e.TestStepRun.Step.FullName;
            result.TimeSpan = TimeSpan.FromSeconds(e.TestStepRun.Result.Duration);
            // result.TestRunner = "Gallio"; // note: can crash in older versions of TD.Net with MissingFieldException

            // It's important to set the stack trace here so the user can double-click in the
            // output window to go the faulting line
            StructuredTestLogStream failureStream = e.TestStepRun.TestLog.GetStream(TestLogStreamNames.Failures);
            if (failureStream != null)
                result.StackTrace = failureStream.ToString();

            StructuredTestLogStream warningStream = e.TestStepRun.TestLog.GetStream(TestLogStreamNames.Warnings);
            if (warningStream != null)
                result.Message = warningStream.ToString();

            // TD.NET will automatically count the number of passed, ignored and failed tests
            // provided we call the TestFinished method with the right State
            result.State = GetTestState(e.TestStepRun.Result.Outcome.Status);

            testListener.TestFinished(result);
        }

        private static FacadeCategory GetCategoryForAnnotation(AnnotationType type)
        {
            switch (type)
            {
                case AnnotationType.Error:
                    return FacadeCategory.Error;

                case AnnotationType.Warning:
                    return FacadeCategory.Warning;

                case AnnotationType.Info:
                    return FacadeCategory.Info;

                default:
                    throw new ArgumentException("type");
            }
        }

        private static FacadeTestState GetTestState(TestStatus status)
        {
            switch (status)
            {
                case TestStatus.Passed:
                    return FacadeTestState.Passed;

                case TestStatus.Inconclusive:
                    return FacadeTestState.Passed;

                case TestStatus.Failed:
                    return FacadeTestState.Failed;

                case TestStatus.Skipped:
                    return FacadeTestState.Ignored;

                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }
    }
}
