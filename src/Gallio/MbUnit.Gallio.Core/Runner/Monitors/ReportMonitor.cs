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
using System.Collections.Generic;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// A test summary monitor tracks <see cref="ITestRunner" /> events and builds
    /// a <see cref="Report" />.
    /// </summary>
    public class ReportMonitor : BaseTestRunnerMonitor
    {
        private readonly Report report;
        private readonly Dictionary<string, StepData> stepDataMap;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public ReportMonitor()
        {
            report = new Report();
            stepDataMap = new Dictionary<string, StepData>();
        }

        /// <summary>
        /// Gets the generated report.
        /// </summary>
        /// <remarks>
        /// The report should be locked if it is being accessed while tests are
        /// running to avoid possible collisions among concurrent threads.
        /// </remarks>
        public Report Report
        {
            get { return report; }
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.LoadPackageComplete += HandleLoadPackageComplete;
            Runner.BuildTemplatesComplete += HandleBuildTemplatesComplete;
            Runner.BuildTestsComplete += HandleBuildTestsComplete;
            Runner.RunStarting += HandleRunStarting;
            Runner.RunComplete += HandleRunComplete;

            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        private void HandleLoadPackageComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.Package = Runner.Package;
                report.TemplateModel = null;
                report.TestModel = null;
                report.PackageRun = null;
            }
        }

        private void HandleBuildTemplatesComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.TemplateModel = Runner.TemplateModel;
                report.TestModel = null;
                report.PackageRun = null;
            }
        }

        private void HandleBuildTestsComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.TestModel = Runner.TestModel;
                report.PackageRun = null;
            }
        }

        private void HandleRunStarting(object sender, EventArgs e)
        {
            lock (report)
            {
                stepDataMap.Clear();

                report.PackageRun = new PackageRun();
                report.PackageRun.StartTime = DateTime.Now;
            }
        }

        private void HandleRunComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                stepDataMap.Clear();

                report.PackageRun.EndTime = DateTime.Now;
                report.PackageRun.Statistics.Duration = (report.PackageRun.EndTime - report.PackageRun.StartTime).TotalSeconds;
            }
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            lock (report)
            {
                switch (e.EventType)
                {
                    case LifecycleEventType.Start:
                    {
                        TestInfo testInfo = Runner.TestModel.Tests[e.StepInfo.TestId];
                        StepRun stepRun = new StepRun(e.StepId, e.StepInfo.Name);
                        StepData stepData = new StepData(testInfo, stepRun);
                        stepDataMap.Add(e.StepId, stepData);

                        if (e.StepInfo.ParentId == null)
                        {
                            TestRun run = new TestRun(e.StepInfo.TestId, stepRun);
                            report.PackageRun.TestRuns.Add(run);
                        }
                        else
                        {
                            StepData parentStepData = GetStepData(e.StepInfo.ParentId);
                            parentStepData.stepRun.Children.Add(stepRun);
                        }

                        stepRun.StartTime = DateTime.Now;
                        break;
                    }

                    case LifecycleEventType.EnterPhase:
                        break;

                    case LifecycleEventType.Finish:
                    {
                        StepData stepData = GetStepData(e.StepId);
                        stepData.stepRun.EndTime = DateTime.Now;
                        stepData.stepRun.Result = e.Result;
                        report.PackageRun.Statistics.MergeStepStatistics(stepData.stepRun, stepData.TestInfo.IsTestCase);

                        stepData.ExecutionLogWriter.Close(); // just in case
                        break;
                    }
                }
            }
        }

        private void HandleExecutionLogEvent(object sender, ExecutionLogEventArgs e)
        {
            lock (report)
            {
                StepData stepData = GetStepData(e.StepId);

                switch (e.EventType)
                {
                    case ExecutionLogEventType.WriteText:
                        stepData.ExecutionLogWriter.WriteText(e.StreamName, e.Text);
                        break;

                    case ExecutionLogEventType.WriteAttachment:
                        stepData.ExecutionLogWriter.WriteAttachment(e.StreamName, e.Attachment);
                        break;

                    case ExecutionLogEventType.BeginSection:
                        stepData.ExecutionLogWriter.BeginSection(e.StreamName, e.SectionName);
                        break;

                    case ExecutionLogEventType.EndSection:
                        stepData.ExecutionLogWriter.EndSection(e.StreamName);
                        break;

                    case ExecutionLogEventType.Close:
                        stepData.ExecutionLogWriter.Close();
                        break;
                }
            }
        }

        private StepData GetStepData(string stepId)
        {
            return stepDataMap[stepId];
        }

        private sealed class StepData
        {
            public readonly TestInfo TestInfo;
            public readonly StepRun stepRun;
            public readonly ExecutionLogWriter ExecutionLogWriter;

            public StepData(TestInfo testInfo, StepRun stepRun)
            {
                TestInfo = testInfo;
                this.stepRun = stepRun;

                ExecutionLogWriter = new ExecutionLogWriter();
                stepRun.ExecutionLog = ExecutionLogWriter.ExecutionLog;
            }
        }
    }
}