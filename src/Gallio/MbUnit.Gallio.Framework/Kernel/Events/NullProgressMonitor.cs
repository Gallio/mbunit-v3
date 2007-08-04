using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A progress monitor that simply validates its parameters but does
    /// not perform any processing.
    /// </summary>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    public class NullProgressMonitor : BaseProgressMonitor
    {
    }
}
