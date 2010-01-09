// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.MSBuildTasks
{
    /// <exclude />
    /// <summary>
    /// Logs messages to a <see cref="TaskLoggingHelper" /> instance
    /// for test results.
    /// </summary>
    internal class TaskLogExtension : LogExtension
    {
        private readonly TaskLoggingHelper taskLoggingHelper;

        public TaskLogExtension(TaskLoggingHelper taskLoggingHelper)
        {
            if (taskLoggingHelper == null)
                throw new ArgumentNullException("taskLoggingHelper");

            this.taskLoggingHelper = taskLoggingHelper;
        }

        protected override void LogAnnotation(AnnotationData annotation)
        {
            StringBuilder message = new StringBuilder();
            message.Append(annotation.Message);

            if (annotation.Type == AnnotationType.Info && annotation.CodeLocation != CodeLocation.Unknown)
            {
                message.Append("\nLocation: ");
                message.Append(annotation.CodeLocation);
            }

            if (annotation.CodeLocation.Line == 0 && annotation.CodeReference != CodeReference.Unknown)
            {
                message.Append("\nReference: ");
                message.Append(annotation.CodeReference);
            }

            if (!string.IsNullOrEmpty(annotation.Details))
            {
                message.Append("\nDetails: ");
                message.Append(annotation.Details);
            }

            switch (annotation.Type)
            {
                case AnnotationType.Info:
                    taskLoggingHelper.LogMessage(MessageImportance.Normal, message.ToString());
                    break;

                case AnnotationType.Error:
                    taskLoggingHelper.LogError(null, null, null, annotation.CodeLocation.Path,
                        annotation.CodeLocation.Line, annotation.CodeLocation.Column, 0, 0, message.ToString());
                    break;

                case AnnotationType.Warning:
                    taskLoggingHelper.LogWarning(null, null, null, annotation.CodeLocation.Path,
                        annotation.CodeLocation.Line, annotation.CodeLocation.Column, 0, 0, message.ToString());
                    break;
            }
        }

        protected override void LogTestCaseStarted(TestStepStartedEventArgs e)
        {
        }

        protected override void LogTestCaseFinished(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }

        protected override void LogNonTestCaseProblem(TestStepFinishedEventArgs e)
        {
            LogTest(e);
        }

        private void LogTest(TestStepFinishedEventArgs e)
        {
            CodeLocation codeLocation = e.TestStepRun.Step.CodeLocation;
            TestOutcome outcome = e.TestStepRun.Result.Outcome;
            string description = String.Format("[{0}] {1} {2}", outcome.DisplayName, e.GetStepKind(), e.TestStepRun.Step.FullName);

            // Note: We exclude code location column information since it is not very useful in the build output.
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    taskLoggingHelper.LogMessage(MessageImportance.Normal, description);
                    break;

                case TestStatus.Failed:
                    taskLoggingHelper.LogError(null, null, null, codeLocation.Path, codeLocation.Line, codeLocation.Column, 0, 0, description);
                    break;

                case TestStatus.Skipped:
                case TestStatus.Inconclusive:
                    taskLoggingHelper.LogWarning(null, null, null, codeLocation.Path, codeLocation.Line, codeLocation.Column, 0, 0, description);
                    break;
            }
        }
    }
}
