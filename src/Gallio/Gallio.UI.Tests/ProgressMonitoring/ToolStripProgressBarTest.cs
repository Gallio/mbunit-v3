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

using System.Windows.Forms;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using ToolStripProgressBar = Gallio.UI.ProgressMonitoring.ToolStripProgressBar;

namespace Gallio.UI.Tests.ProgressMonitoring
{
    public class ToolStripProgressBarTest
    {
        private ToolStripProgressBar toolStripProgressBar;
        private ObservableProgressMonitor progressMonitor;

        [SetUp]
        public void SetUp()
        {
            toolStripProgressBar = new ToolStripProgressBar();
            progressMonitor = new ObservableProgressMonitor();
        }

        [Test]
        public void Maximum_should_be_total_work_units()
        {
            const int totalWorkUnits = 5;
            progressMonitor.BeginTask("blahblahblah", totalWorkUnits);

            toolStripProgressBar.ProgressChanged(progressMonitor);

            Assert.AreEqual(totalWorkUnits, toolStripProgressBar.Maximum);
        }

        [Test]
        public void Value_should_be_completed_work_units()
        {
            progressMonitor.BeginTask("blahblahblah", 5);
            const int completedWorkUnits = 3;           
            progressMonitor.Worked(completedWorkUnits);

            toolStripProgressBar.ProgressChanged(progressMonitor);

            Assert.AreEqual(completedWorkUnits, toolStripProgressBar.Value);
        }

        [Test]
        public void Style_should_be_continuous()
        {
            progressMonitor.BeginTask("blahblahblah", 5);

            toolStripProgressBar.ProgressChanged(progressMonitor);

            Assert.AreEqual(ProgressBarStyle.Continuous, toolStripProgressBar.Style);
        }

        [Test]
        public void Style_should_be_marquee_if_total_work_units_is_NaN()
        {
            progressMonitor.BeginTask("blahblahblah", double.NaN);

            toolStripProgressBar.ProgressChanged(progressMonitor);

            Assert.AreEqual(ProgressBarStyle.Marquee, toolStripProgressBar.Style);
        }

        [Test]
        public void Maximum_should_be_zero_if_work_is_done()
        {
            using (progressMonitor.BeginTask("blahblahblah", 1)) { }

            toolStripProgressBar.ProgressChanged(progressMonitor);

            Assert.AreEqual(0, toolStripProgressBar.Maximum);
        }
    }
}
