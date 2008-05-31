// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Tests;
using Gallio.Runtime.ProgressMonitoring;

using MbUnit.Framework;

using Rhino.Mocks;
using System;

namespace Gallio.Icarus.Core.ProgressMonitoring.Tests
{
    [TestFixture]
    public class StatusStripProgressMonitorPresenterTest : MockTest
    {
        [Test]
        public void Canceled_Test()
        {
            IProjectPresenter projectPresenter = mocks.CreateMock<IProjectPresenter>();
            projectPresenter.TaskName = "Operation cancelled";
            projectPresenter.SubTaskName = String.Empty;
            projectPresenter.CompletedWorkUnits = 0;
            projectPresenter.TotalWorkUnits = 0;
            mocks.ReplayAll();
            StatusStripProgressMonitorPresenter statusStripProgressMonitorPresenter = new StatusStripProgressMonitorPresenter(projectPresenter);
            ObservableProgressMonitor observableProgressMonitor = new ObservableProgressMonitor();
            statusStripProgressMonitorPresenter.Present(observableProgressMonitor);
            observableProgressMonitor.Cancel();
        }

        [Test]
        public void HandleChanged_Test()
        {
            IProjectPresenter projectPresenter = mocks.CreateMock<IProjectPresenter>();
            projectPresenter.TotalWorkUnits = 10;
            Expect.Call(projectPresenter.CompletedWorkUnits = 0).Repeat.Twice();
            projectPresenter.TaskName = "taskName";
            projectPresenter.SubTaskName = String.Empty;
            mocks.ReplayAll();
            StatusStripProgressMonitorPresenter statusStripProgressMonitorPresenter = new StatusStripProgressMonitorPresenter(projectPresenter);
            ObservableProgressMonitor observableProgressMonitor = new ObservableProgressMonitor();
            statusStripProgressMonitorPresenter.Present(observableProgressMonitor);
            observableProgressMonitor.BeginTask("taskName", 10);
        }
    }
}
