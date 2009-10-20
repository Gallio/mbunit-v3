using System;
using System.ComponentModel;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.ProgressMonitoring
{
    internal class Win7TaskBar
    {
        private readonly IntPtr windowHandle;
        private readonly ITestController testController;
        private readonly ITaskbarList4 taskBarList;
        private TBPFLAG currentProgressState;

        public Win7TaskBar(IntPtr windowHandle, ITaskbarList4 taskBarList, 
            ITestController testController)
        {
            this.windowHandle = windowHandle;
            
            this.testController = testController;
            testController.PropertyChanged += UpdateTaskbar;

            this.taskBarList = taskBarList;
            taskBarList.HrInit();
        }

        private void UpdateTaskbar(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Passed" && e.PropertyName != "Failed" &&
                e.PropertyName != "Skipped" && e.PropertyName != "Inconclusive")
                return;

            TBPFLAG newProgressState;
            if (testController.Failed > 0)
                newProgressState = TBPFLAG.TBPF_ERROR; // red
            else if (testController.Skipped > 0)
                newProgressState = TBPFLAG.TBPF_PAUSED; // yellow
            else if (testController.Passed > 0)
                newProgressState = TBPFLAG.TBPF_NORMAL; // green
            else
                newProgressState = TBPFLAG.TBPF_NOPROGRESS;

            if (newProgressState != currentProgressState)
            {
                currentProgressState = newProgressState;
                taskBarList.SetProgressState(windowHandle, newProgressState);
            }

            var completed = testController.Passed + testController.Failed +
                testController.Skipped + testController.Inconclusive;

            var total = completed > testController.TestCount ? completed 
                : testController.TestCount;

            taskBarList.SetProgressValue(windowHandle, Convert.ToUInt32(completed), 
                Convert.ToUInt32(total));
        }
    }
}
