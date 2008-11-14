using System;

namespace Gallio.Icarus.Utilities
{
    public interface IUnhandledExceptionPolicy
    {
        void Report(string message, Exception unhandledException);
    }
}
