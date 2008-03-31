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
using Gallio.Icarus.Core.Interfaces;
using Gallio.Framework;
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

        public TestRunnerMonitor(IProjectPresenter presenter, ReportMonitor reportMonitor)
        {
            if (presenter == null)
                throw new ArgumentNullException(@"presenter");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.presenter = presenter;
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
            // Ignore tests that aren't test cases.
            // FIXME: This code is not a good idea because it will cause failures and other
            //        information recorded about non-test cases (like fixtures) to be disregarded
            //        which could make it very difficult for a user to understand what broke.
            if (!e.TestStepRun.Step.IsTestCase)
                return;

            presenter.Update(e.Test, e.TestStepRun);
        }
    }
}