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
using System.Text;
using Castle.Core.Logging;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Monitors
{
    /// <summary>
    /// A log monitor writes a summary of test execution progress to an <see cref="ILogger" />
    /// so the user can monitor what's going on.  Passing tests are recorded with severity
    /// <see cref="LoggerLevel.Info" />, warnings are recorded with severity <see cref="LoggerLevel.Warn" />
    /// and failures are recorded with severity <see cref="LoggerLevel.Error" />.
    /// </summary>
    public class LogMonitor : BaseTestRunnerMonitor
    {
        private readonly ReportMonitor reportMonitor;
        private readonly ILogger logger;

        /// <summary>
        /// Creates a log monitor.
        /// </summary>
        /// <param name="logger">The logger to which messages should be written</param>
        /// <param name="reportMonitor">The report monitor to use to obtain test results</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/>
        /// or <paramref name="reportMonitor"/> is null</exception>
        public LogMonitor(ILogger logger, ReportMonitor reportMonitor)
        {
            if (logger == null)
                throw new ArgumentNullException(@"logger");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.logger = logger;
            this.reportMonitor = reportMonitor;
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            reportMonitor.TestStepStarting += HandleStepStarting;
            reportMonitor.TestStepFinished += HandleStepFinished;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            reportMonitor.TestStepStarting -= HandleStepStarting;
            reportMonitor.TestStepFinished -= HandleStepFinished;
        }

        private void HandleStepStarting(object sender, TestStepRunEventArgs e)
        {
            logger.DebugFormat("[starting] {0}", e.TestStepRun.Step.FullName);
        }

        private void HandleStepFinished(object sender, TestStepRunEventArgs e)
        {
            TestOutcome outcome = e.TestStepRun.Result.Outcome;
            LoggerLevel level = GetLoggerLevelForOutcome(outcome);
            string warnings = FormatStream(e.TestStepRun, LogStreamNames.Warnings);
            string failures = FormatStream(e.TestStepRun, LogStreamNames.Failures);

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("[{0}] {1}", outcome.DisplayName, e.GetStepKind());

            if (warnings.Length != 0)
            {
                if (level < LoggerLevel.Warn)
                    level = LoggerLevel.Warn;

                messageBuilder.AppendLine();
                messageBuilder.Append(warnings);
                messageBuilder.AppendLine();
            }

            if (failures.Length != 0)
            {
                if (level < LoggerLevel.Error)
                    level = LoggerLevel.Error;

                messageBuilder.AppendLine();
                messageBuilder.Append(failures);
                messageBuilder.AppendLine();
            }

            // Exclude nested test steps from the results except as debug messages.
            if (e.TestStepRun.Step.ParentId != null)
                level = LoggerLevel.Debug;

            Log(level, messageBuilder.ToString());
        }

        private void Log(LoggerLevel level, string message)
        {
            switch (level)
            {
                case LoggerLevel.Info:
                    logger.Info(message);
                    break;
                case LoggerLevel.Warn:
                    logger.Warn(message);
                    break;
                case LoggerLevel.Error:
                    logger.Error(message);
                    break;
                case LoggerLevel.Debug:
                    logger.Debug(message);
                    break;
            }
        }

        private static string FormatStream(TestStepRun testStepRun, string streamName)
        {
            ExecutionLogStream stream = testStepRun.ExecutionLog.GetStream(streamName);
            return stream != null ? stream.ToString() : @"";
        }

        private static LoggerLevel GetLoggerLevelForOutcome(TestOutcome outcome)
        {
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return LoggerLevel.Info;

                case TestStatus.Skipped:
                case TestStatus.Inconclusive:
                    return LoggerLevel.Warn;

                case TestStatus.Failed:
                    return LoggerLevel.Error;

                default:
                    throw new ArgumentException("outcome");
            }
        }
    }
}
