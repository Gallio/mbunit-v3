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
