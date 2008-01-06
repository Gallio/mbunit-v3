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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Monitors;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.MSBuildTasks
{
    /// <exclude />
    /// <summary>
    /// Logs messages to a <see cref="TaskLoggingHelper" /> instance
    /// for test results.
    /// </summary>
    internal class TaskTestRunnerMonitor : BaseTestRunnerMonitor
    {
        private readonly TaskLoggingHelper taskLoggingHelper;
        private readonly ReportMonitor reportMonitor;

        public TaskTestRunnerMonitor(TaskLoggingHelper taskLoggingHelper, ReportMonitor reportMonitor)
        {
            if (taskLoggingHelper == null)
                throw new ArgumentNullException("taskLoggingHelper");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.taskLoggingHelper = taskLoggingHelper;
            this.reportMonitor = reportMonitor;
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            reportMonitor.TestStepFinished += HandleStepFinished;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            reportMonitor.TestStepFinished -= HandleStepFinished;
        }

        private void HandleStepFinished(object sender, TestStepRunEventArgs e)
        {
            if (e.TestStepRun.Step.ParentId != null)
                return;

            CodeLocation codeLocation = e.TestStepRun.Step.CodeLocation
                ?? new CodeLocation(@"(unknown)", 0, 0);

            string testKind = e.TestStepRun.Step.Metadata.GetValue(MetadataKeys.TestKind)
                ?? e.TestInstanceRun.TestInstance.Metadata.GetValue(MetadataKeys.TestKind)
                ?? e.TestData.Metadata.GetValue(MetadataKeys.TestKind)
                ?? TestKinds.Test;

            LogOutcome(testKind, e.TestStepRun.Step.FullName, codeLocation, e.TestStepRun.Result.Outcome, e.TestStepRun.Result.Status);
        }

        private void LogOutcome(string testKind, string testName, CodeLocation codeLocation, TestOutcome outcome, TestStatus status)
        {
            // Note: We exclude column information since it is not very useful in the build output.
            switch (outcome)
            {
                case TestOutcome.Passed:
                    taskLoggingHelper.LogMessage(MessageImportance.Normal, "{0} '{1}' passed.", testKind, testName);
                    break;

                case TestOutcome.Failed:
                    taskLoggingHelper.LogError(null, null, null,
                        codeLocation.Path, codeLocation.Line, 0, 0, 0,
                        "{0} '{1}' failed.", testKind, testName);
                    break;

                case TestOutcome.Inconclusive:
                    string message;
                    switch (status)
                    {
                        case TestStatus.Canceled:
                            message = "was canceled";
                            break;

                        case TestStatus.Error:
                            message = "encountered an error";
                            break;

                        default:
                        case TestStatus.Executed:
                            message = "was inconclusive";
                            break;

                        case TestStatus.Ignored:
                            message = "was ignored";
                            break;

                        case TestStatus.NotRun:
                            message = "was not run";
                            break;

                        case TestStatus.Skipped:
                            message = "was skipped";
                            break;
                    }

                    taskLoggingHelper.LogWarning(null, null, null,
                        codeLocation.Path, codeLocation.Line, 0, 0, 0,
                        "{0} '{1}' {2}.", testKind, testName, message);
                    break;
            }
        }
    }
}
