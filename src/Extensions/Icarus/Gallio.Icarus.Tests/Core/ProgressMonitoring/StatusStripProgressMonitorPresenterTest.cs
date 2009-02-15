// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Core.CustomEventArgs;

namespace Gallio.Icarus.Core.ProgressMonitoring.Tests
{
    [TestFixture]
    public class StatusStripProgressMonitorPresenterTest
    {
        [Test]
        public void Canceled_Test()
        {
            ProgressUpdateEventArgs progressUpdateEventArgs = null;
            StatusStripProgressMonitorPresenter statusStripProgressMonitorPresenter = new StatusStripProgressMonitorPresenter();
            statusStripProgressMonitorPresenter.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e) { progressUpdateEventArgs = e; };
            ObservableProgressMonitor observableProgressMonitor = new ObservableProgressMonitor();
            statusStripProgressMonitorPresenter.Present(observableProgressMonitor);
            observableProgressMonitor.Cancel();
            Assert.AreEqual("Operation cancelled", progressUpdateEventArgs.TaskName);
            Assert.AreEqual(string.Empty, progressUpdateEventArgs.SubTaskName);
            Assert.AreEqual(0, progressUpdateEventArgs.CompletedWorkUnits);
            Assert.AreEqual(0, progressUpdateEventArgs.TotalWorkUnits);
        }

        [Test]
        public void HandleChanged_Test()
        {
            ProgressUpdateEventArgs progressUpdateEventArgs = null;
            StatusStripProgressMonitorPresenter statusStripProgressMonitorPresenter = new StatusStripProgressMonitorPresenter();
            statusStripProgressMonitorPresenter.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e) { progressUpdateEventArgs = e; };
            ObservableProgressMonitor observableProgressMonitor = new ObservableProgressMonitor();
            statusStripProgressMonitorPresenter.Present(observableProgressMonitor);
            observableProgressMonitor.BeginTask("taskName", 10);
            Assert.AreEqual("taskName", progressUpdateEventArgs.TaskName);
            Assert.AreEqual(string.Empty, progressUpdateEventArgs.SubTaskName);
            Assert.AreEqual(0, progressUpdateEventArgs.CompletedWorkUnits);
            Assert.AreEqual(10, progressUpdateEventArgs.TotalWorkUnits);
        }
    }
}
