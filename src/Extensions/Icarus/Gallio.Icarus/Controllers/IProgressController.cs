using System;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.ProgressMonitoring.EventArgs;

namespace Gallio.Icarus.Controllers
{
    interface IProgressController
    {
        event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        event EventHandler<DisplayProgressDialogEventArgs> DisplayProgressDialog;

        void Cancel();
    }
}
