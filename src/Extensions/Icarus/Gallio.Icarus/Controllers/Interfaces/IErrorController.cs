using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime;

namespace Gallio.Icarus.Controllers.Interfaces
{
    interface IErrorController
    {
        List<Exception> Errors { get; }
        void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e);
    }
}
