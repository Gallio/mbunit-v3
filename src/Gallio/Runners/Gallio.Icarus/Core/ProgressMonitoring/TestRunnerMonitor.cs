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
using System.Collections.Generic;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    public class TestRunnerMonitor : BaseTestRunnerMonitor
    {
        private readonly ReportMonitor reportMonitor;
        private readonly IProjectPresenter presenter;
        private Dictionary<string, Dictionary<string, string>> logStreams;

        public TestRunnerMonitor(IProjectPresenter presenter, ReportMonitor reportMonitor)
        {
            if (presenter == null)
                throw new ArgumentNullException(@"presenter");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.presenter = presenter;
            this.reportMonitor = reportMonitor;
            logStreams = new Dictionary<string, Dictionary<string, string>>();
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
            // Ignore tests that aren't test cases & nested test steps.
            if (!e.TestData.IsTestCase || e.TestStepRun.Step.ParentId != null)
                return;

            presenter.Update(e.TestData, e.TestStepRun);

            // store log streams
            foreach (ExecutionLogStream els in e.TestStepRun.ExecutionLog.Streams)
            {
                string key = els.Name + e.TestData.Id;
                if (logStreams.ContainsKey(e.TestData.Id))
                {
                    if (logStreams[e.TestData.Id].ContainsKey(els.Name))
                        logStreams[e.TestData.Id][els.Name] += els.ToString();
                    else
                        logStreams[e.TestData.Id].Add(els.Name, els.ToString());
                }
                else
                {
                    Dictionary<string, string> logs = new Dictionary<string, string>();
                    logs.Add(els.Name, els.ToString());
                    logStreams.Add(e.TestData.Id, logs);
                }
            }
        }

        public IList<string> GetAvailableLogStreams(string testId)
        {
            List<string> logs = new List<string>();
            if (logStreams.ContainsKey(testId))
                logs.AddRange(logStreams[testId].Keys);
            return logs;
        }

        public string GetLogStream(string logStream, string testId)
        {
            if (logStreams.ContainsKey(testId))
                if (logStreams[testId].ContainsKey(logStream))
                    return logStreams[testId][logStream];
            return string.Empty;
        }
    }
}