using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime;

namespace Gallio.Icarus.Controllers
{
    class ErrorController
    {
        public List<Exception> Errors
        {
            get;
            private set;
        }

        public ErrorController()
        {
            Errors = new List<Exception>();
        }

        public void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
        {
            Errors.Add(e.Exception);
        }
    }
}
