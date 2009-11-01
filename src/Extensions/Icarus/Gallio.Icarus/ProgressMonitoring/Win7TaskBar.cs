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

using System;
using System.ComponentModel;
using Gallio.Icarus.Models;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.ProgressMonitoring
{
    internal class Win7TaskBar
    {
        private readonly IntPtr windowHandle;
        private readonly ITestTreeModel testTreeModel;
        private readonly ITestStatistics testStatistics;
        private readonly ITaskbarList4 taskBarList;
        private TBPFLAG currentProgressState;

        public Win7TaskBar(IntPtr windowHandle, ITaskbarList4 taskBarList, ITestTreeModel testTreeModel, 
            ITestStatistics testStatistics)
        {
            this.windowHandle = windowHandle;
            
            this.testTreeModel = testTreeModel;

            this.testStatistics = testStatistics;
            testStatistics.Passed.PropertyChanged += UpdateTaskbar;
            testStatistics.Failed.PropertyChanged += UpdateTaskbar;
            testStatistics.Skipped.PropertyChanged += UpdateTaskbar;
            testStatistics.Inconclusive.PropertyChanged += UpdateTaskbar;

            this.taskBarList = taskBarList;
            taskBarList.HrInit();
        }

        private void UpdateTaskbar(object sender, PropertyChangedEventArgs e)
        {
            TBPFLAG newProgressState;
            if (testStatistics.Failed > 0)
                newProgressState = TBPFLAG.TBPF_ERROR; // red
            else if (testStatistics.Skipped > 0)
                newProgressState = TBPFLAG.TBPF_PAUSED; // yellow
            else if (testStatistics.Passed > 0)
                newProgressState = TBPFLAG.TBPF_NORMAL; // green
            else
                newProgressState = TBPFLAG.TBPF_NOPROGRESS;

            if (newProgressState != currentProgressState)
            {
                currentProgressState = newProgressState;
                taskBarList.SetProgressState(windowHandle, newProgressState);
            }

            var completed = testStatistics.Passed + testStatistics.Failed +
                testStatistics.Skipped + testStatistics.Inconclusive;

            var total = completed > testTreeModel.TestCount ? completed 
                : testTreeModel.TestCount;

            taskBarList.SetProgressValue(windowHandle, Convert.ToUInt32(completed), 
                Convert.ToUInt32(total));
        }
    }
}
