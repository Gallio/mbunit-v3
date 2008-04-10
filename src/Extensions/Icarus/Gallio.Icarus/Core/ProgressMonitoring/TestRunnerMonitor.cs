// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;

using Gallio.Icarus.Core.Interfaces;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    public class TestRunnerMonitor : BaseTestRunnerMonitor
    {
        private readonly ReportMonitor reportMonitor;
        private readonly IProjectPresenter presenter;
        private readonly string reportFolder;

        public TestRunnerMonitor(IProjectPresenter presenter, ReportMonitor reportMonitor, string reportFolder)
        {
            if (presenter == null)
                throw new ArgumentNullException(@"presenter");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");
            if (string.IsNullOrEmpty(reportFolder))
                throw new ArgumentException(@"reportFolder");

            this.presenter = presenter;
            this.reportMonitor = reportMonitor;
            this.reportFolder = reportFolder;
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
            presenter.Update(e.Test, e.TestStepRun);

            // store attachments as we go along for the execution log viewer!
            string attachmentDirectory = string.Empty;
            if (e.TestStepRun.ExecutionLog.Attachments.Count > 0)
            {
                attachmentDirectory = Path.Combine(reportFolder, FileUtils.EncodeFileName(e.TestStepRun.Step.Id));
                if (!Directory.Exists(attachmentDirectory))
                    Directory.CreateDirectory(attachmentDirectory);
            }

            foreach (ExecutionLogAttachment ela in e.TestStepRun.ExecutionLog.Attachments)
            {
                string fileName = Path.Combine(attachmentDirectory, FileUtils.EncodeFileName(ela.Name));
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    ela.SaveContents(fs, Encoding.Default);
            }
        }
    }
}