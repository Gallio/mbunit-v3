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

using System;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Utilities;
using System.Text;

namespace Gallio.Icarus
{
    public partial class ProgressMonitor : Form
    {
        private readonly ITestController testController;

        public ProgressMonitor()
        {
            this.testController = testController;

            InitializeComponent();

            testController.ProgressUpdate += testController_ProgressUpdate;
        }

        private void testController_ProgressUpdate(object sender, ProgressMonitoring.EventArgs.ProgressUpdateEventArgs e)
        {
            Sync.Invoke(this, () =>
            {
                progressBar.Maximum = Convert.ToInt32(e.TotalWorkUnits);
                progressBar.Value = Convert.ToInt32(e.CompletedWorkUnits);
                Text = e.TaskName;
                subTaskNameLabel.Text = e.SubTaskName;
                percentLabel.Text = (e.TotalWorkUnits > 0)
                    ? String.Format("({0:P0})", e.CompletedWorkUnits / e.TotalWorkUnits)
                    : String.Empty;
            });
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            testController.Cancel();
        }

        private void runInBackgroundButton_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ((Main) ParentForm).ShowProgressMonitor = false;
            Hide();
        }
    }
}
