using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Icarus.Utilities
{
    public class UnhandledExceptionPolicy : IUnhandledExceptionPolicy
    {
        public void Report(string message, Exception unhandledException)
        {
            Gallio.Runtime.UnhandledExceptionPolicy.Report(message, unhandledException);
        }
    }
}
