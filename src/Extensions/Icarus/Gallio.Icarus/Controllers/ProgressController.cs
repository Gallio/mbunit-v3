using System;
using Gallio.Common.Policies;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using System.Timers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Controllers.EventArgs;

namespace Gallio.Icarus.Controllers
{
    internal class ProgressController : IProgressController
    {
        private readonly ITaskManager taskManager;
        private readonly Timer timer = new Timer();

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler<DisplayProgressDialogEventArgs> DisplayProgressDialog;

        public ProgressController(ITaskManager taskManager, IOptionsController optionsController)
        {
            this.taskManager = taskManager;

            bool displayProgressDialog = false;
            timer.AutoReset = false;
            timer.Interval = 3000;
            timer.Elapsed += (sender, e) => { displayProgressDialog = true; };

            taskManager.ProgressUpdate += (sender, e) =>
            {
                if (displayProgressDialog)
                {
                    EventHandlerPolicy.SafeInvoke(DisplayProgressDialog, this, 
                        new DisplayProgressDialogEventArgs(taskManager.ProgressMonitor, e));
                    displayProgressDialog = false;
                }
                EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, e);
            };
            
            if (optionsController.ShowProgressDialogs)
                taskManager.TaskStarted += (sender, e) => timer.Start();
        }

        public void Cancel()
        {
            // cancel any running work
            if (taskManager.ProgressMonitor != null)
                taskManager.ProgressMonitor.Cancel();

            // remove anything else in the queue
            taskManager.Stop();
        }
    }
}
