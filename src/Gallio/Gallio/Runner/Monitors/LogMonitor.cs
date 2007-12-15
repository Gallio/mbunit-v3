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
using System.Text;
using Castle.Core.Logging;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Properties;
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
            logger.DebugFormat(Resources.LogMonitor_HeaderFormat,
                Resources.LogMonitor_Status_Starting, e.TestStepRun.Step.FullName);
        }

        private void HandleStepFinished(object sender, TestStepRunEventArgs e)
        {
            LoggerLevel level;
            string status = GetFinishedMessageStatus(e.TestStepRun.Result.Outcome, e.TestStepRun.Result.Status, out level);
            string warnings = FormatStream(e.TestStepRun, LogStreamNames.Warnings);
            string failures = FormatStream(e.TestStepRun, LogStreamNames.Failures);

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat(Resources.LogMonitor_HeaderFormat, status, e.TestStepRun.Step.FullName);

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
            }
        }

        private static string FormatStream(TestStepRun testStepRun, string streamName)
        {
            ExecutionLogStream stream = testStepRun.ExecutionLog.GetStream(streamName);
            return stream != null ? stream.ToString() : @"";
        }

        private static string GetFinishedMessageStatus(TestOutcome outcome, TestStatus status, out LoggerLevel level)
        {
            switch (outcome)
            {
                case TestOutcome.Passed:
                    level = LoggerLevel.Info;
                    return Resources.LogMonitor_Status_Passed;

                case TestOutcome.Failed:
                    level = LoggerLevel.Error;
                    return Resources.LogMonitor_Status_Failed;

                case TestOutcome.Inconclusive:
                    level = LoggerLevel.Info;

                    switch (status)
                    {
                        case TestStatus.Canceled:
                            return Resources.LogMonitor_Status_Canceled;
                        case TestStatus.Error:
                            return Resources.LogMonitor_Status_Error;
                        case TestStatus.Executed:
                            return Resources.LogMonitor_Status_Inconclusive;
                        case TestStatus.Ignored:
                            return Resources.LogMonitor_Status_Ignored;
                        case TestStatus.NotRun:
                            return Resources.LogMonitor_Status_NotRun;
                        case TestStatus.Skipped:
                            return Resources.LogMonitor_Status_Skipped;
                    }
                    break;
            }

            level = LoggerLevel.Error;
            return Resources.LogMonitor_Status_Unknown;
        }
    }
}
