using System;
using Gallio.UI.DataBinding;

namespace Gallio.Copy
{
    internal interface IProgressController
    {
        event EventHandler<ProgressEvent> DisplayProgressDialog;
        Observable<string> Status { get; }
        Observable<double> TotalWork { get; }
        Observable<double> CompletedWork { get; }
        void Cancel();
    }
}