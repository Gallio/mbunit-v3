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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.ExecutionLog
{
    internal partial class ExecutionLogWindow : UserControl
    {
        public ExecutionLogWindow(IExecutionLogController executionLogController, IOptionsController optionsController)
        {
            InitializeComponent();

            EventHandler<ExecutionLogUpdatedEventArgs> onExecutionLogUpdated = (s, e) => reportViewer.Show(e.TestStepRuns, optionsController.RecursiveExecutionLog);
            executionLogController.ExecutionLogUpdated += onExecutionLogUpdated;
            Disposed += (s, e) => executionLogController.ExecutionLogUpdated -= onExecutionLogUpdated;

            EventHandler<EventArgs> onExecutionLogReset = (s, e) => reportViewer.Clear();
            executionLogController.ExecutionLogReset += onExecutionLogReset;
            Disposed += (s, e) => executionLogController.ExecutionLogReset -= onExecutionLogReset;
        }
    }
}