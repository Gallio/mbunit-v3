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

            TestOutcome outcome = e.TestStepRun.Result.Outcome;
            string description = String.Format("{0} '{1}' {2}.", e.GetStepKind(), e.TestStepRun.Step.FullName, outcome.DisplayName);

            // Note: We exclude code location column information since it is not very useful in the build output.
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    taskLoggingHelper.LogMessage(MessageImportance.Normal, description);
                    break;

                case TestStatus.Failed:
                    taskLoggingHelper.LogError(null, null, null, codeLocation.Path, codeLocation.Line, 0, 0, 0, description);
                    break;

                case TestStatus.Skipped:
                case TestStatus.Inconclusive:
                    taskLoggingHelper.LogWarning(null, null, null, codeLocation.Path, codeLocation.Line, 0, 0, 0, description);
                    break;
            }
        }
    }
}
