using System;
using System.IO;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IExecutionLogController
    {
        Stream ExecutionLog { get; }
        string ExecutionLogFolder { get; set; }

        event EventHandler<System.EventArgs> ExecutionLogUpdated;
    }
}
