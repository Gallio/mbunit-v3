using System;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Copy
{
    internal class ProgressEvent : EventArgs
    {
        public ObservableProgressMonitor ProgressMonitor { get; private set; }

        public ProgressEvent(ObservableProgressMonitor progressMonitor)
        {
            ProgressMonitor = progressMonitor;
        }
    }
}