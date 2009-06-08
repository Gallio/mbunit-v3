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
using Gallio.Common.Collections;
using Gallio.Common.Markup;
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// The log extension writes a summary of test execution progress to the test runner's logger
    /// so the user can monitor what's going on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Passing tests are recorded with severity <see cref="LogSeverity.Info" />, warnings are
    /// recorded with severity <see cref="LogSeverity.Warning" /> and failures are recorded
    /// with severity <see cref="LogSeverity.Error" />.
    /// </para>
    /// </remarks>
    public class LogExtension : TestRunnerExtension
    {
        /// <inheritdoc />
        protected override void Initialize()
        {
            var testCaseSteps = new HashSet<string>();

            Events.TestModelAnnotationAdded += delegate(object sender, TestModelAnnotationAddedEventArgs e)
            {
                LogAnnotation(e.Annotation);
            };

            Events.TestStepStarted += delegate(object sender, TestStepStartedEventArgs e)
            {
                if (e.TestStepRun.Step.IsTestCase)
                {
                    testCaseSteps.Add(e.TestStepRun.Step.Id);

                    LogTestCaseStarted(e);
                }
                else
                {
                    string parentId = e.TestStepRun.Step.ParentId;
                    if (parentId != null && testCaseSteps.Contains(parentId))
                        testCaseSteps.Add(e.TestStepRun.Step.Id);
                }
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                if (e.TestStepRun.Step.IsTestCase)
                {
                    testCaseSteps.Remove(e.TestStepRun.Step.Id);

                    LogTestCaseFinished(e);
                }
                else
                {
                    if (!testCaseSteps.Contains(e.TestStepRun.Step.Id))
                    {
                        if (e.TestStepRun.Result.Outcome.Status != TestStatus.Passed
                            && (e.TestStepRun.TestLog.GetStream(MarkupStreamNames.Warnings) != null
                                || e.TestStepRun.TestLog.GetStream(MarkupStreamNames.Failures) != null))
                        {
                            LogNonTestCaseProblem(e);
                        }
                    }
                    else
                    {
                        testCaseSteps.Remove(e.TestStepRun.Step.Id);
                    }
                }

            };
        }

        /// <summary>
        /// Logs an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to log.</param>
        protected virtual void LogAnnotation(AnnotationData annotation)
        {
            annotation.Log(Logger, true);
        }

        /// <summary>
        /// Logs a message about a test case that has started.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is not called for test steps that have <see cref="ITestStep.IsTestCase"/> set to false.
        /// </para>
        /// </remarks>
        /// <param name="e">The event.</param>
        protected virtual void LogTestCaseStarted(TestStepStartedEventArgs e)
        {
            Logger.Log(LogSeverity.Debug, String.Format("[starting] {0}", e.TestStepRun.Step.FullName));
        }

        /// <summary>
        /// Logs a message about a test case that has finished.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is not called for test steps that have <see cref="ITestStep.IsTestCase"/> set to false.
        /// </para>
        /// </remarks>
        /// <param name="e">The event.</param>
        protected virtual void LogTestCaseFinished(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }

        /// <summary>
        /// Logs a message about a non-test case that has finished with some problem that
        /// may have prevented other test cases from running correctly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is not called for steps within test cases.
        /// </para>
        /// <para>
        /// This method is not called for test steps that have <see cref="ITestStep.IsTestCase"/> set to true.
        /// </para>
        /// </remarks>
        /// <param name="e">The event.</param>
        protected virtual void LogNonTestCaseProblem(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }
        
        private void LogTest(TestStepFinishedEventArgs e)
        {
            TestOutcome outcome = e.TestStepRun.Result.Outcome;
            LogSeverity severity = GetLogSeverityForOutcome(outcome);
            string warnings = FormatStream(e.TestStepRun, MarkupStreamNames.Warnings);
            string failures = FormatStream(e.TestStepRun, MarkupStreamNames.Failures);

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("[{0}] {1} {2}", outcome.DisplayName, e.GetStepKind(), e.TestStepRun.Step.FullName);

            if (warnings.Length != 0)
            {
                if (severity < LogSeverity.Warning)
                    severity = LogSeverity.Warning;

                messageBuilder.AppendLine();
                messageBuilder.Append(warnings);
                messageBuilder.AppendLine();
            }

            if (failures.Length != 0)
            {
                if (severity < LogSeverity.Error)
                    severity = LogSeverity.Error;

                messageBuilder.AppendLine();
                messageBuilder.Append(failures);
                messageBuilder.AppendLine();
            }

            Logger.Log(severity, messageBuilder.ToString());
        }

        private static string FormatStream(TestStepRun testStepRun, string streamName)
        {
            StructuredStream stream = testStepRun.TestLog.GetStream(streamName);
            return stream != null ? stream.ToString() : @"";
        }

        private static LogSeverity GetLogSeverityForOutcome(TestOutcome outcome)
        {
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return LogSeverity.Info;

                case TestStatus.Skipped:
                case TestStatus.Inconclusive:
                    return LogSeverity.Warning;

                case TestStatus.Failed:
                    return LogSeverity.Error;

                default:
                    throw new ArgumentException("outcome");
            }
        }
    }
}
