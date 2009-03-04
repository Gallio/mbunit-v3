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
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// <para>
    /// The log extension writes a summary of test execution progress to the test runner's logger
    /// so the user can monitor what's going on.
    /// </para>
    /// Passing tests are recorded with severity <see cref="LogSeverity.Info" />, warnings are
    /// recorded with severity <see cref="LogSeverity.Warning" /> and failures are recorded
    /// with severity <see cref="LogSeverity.Error" />.
    /// </summary>
    public class LogExtension : TestRunnerExtension
    {
        /// <inheritdoc />
        protected override void Initialize()
        {
            Events.ExploreFinished += delegate(object sender, ExploreFinishedEventArgs e)
            {
                if (e.Success)
                {
                    foreach (AnnotationData annotation in e.Report.TestModel.Annotations)
                        LogAnnotation(annotation);
                }
            };

            Events.TestStepStarted += delegate(object sender, TestStepStartedEventArgs e)
            {
                if (e.TestStepRun.Step.IsTestCase)
                    LogTestCaseStarted(e);
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                if (e.TestStepRun.Step.IsTestCase)
                    LogTestCaseFinished(e);
            };
        }

        /// <summary>
        /// Logs an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to log</param>
        protected virtual void LogAnnotation(AnnotationData annotation)
        {
            annotation.Log(Logger, true);
        }

        /// <summary>
        /// Logs a message about a test case that has started.
        /// </summary>
        /// <remarks>
        /// This method is not called for test steps that have <see cref="ITestStep.IsTestCase"/> set to false.
        /// </remarks>
        /// <param name="e">The event</param>
        protected virtual void LogTestCaseStarted(TestStepStartedEventArgs e)
        {
            Logger.Log(LogSeverity.Debug, String.Format("[starting] {0}", e.TestStepRun.Step.FullName));
        }

        /// <summary>
        /// Logs a message about a test case that has finished.
        /// </summary>
        /// <remarks>
        /// This method is not called for test steps that have <see cref="ITestStep.IsTestCase"/> set to false.
        /// </remarks>
        /// <param name="e">The event</param>
        protected virtual void LogTestCaseFinished(TestStepFinishedEventArgs e)
        {
            TestOutcome outcome = e.TestStepRun.Result.Outcome;
            LogSeverity severity = GetLogSeverityForOutcome(outcome);
            string warnings = FormatStream(e.TestStepRun, TestLogStreamNames.Warnings);
            string failures = FormatStream(e.TestStepRun, TestLogStreamNames.Failures);

            StringBuilder messageBuilder = new StringBuilder();
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
            StructuredTestLogStream stream = testStepRun.TestLog.GetStream(streamName);
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
