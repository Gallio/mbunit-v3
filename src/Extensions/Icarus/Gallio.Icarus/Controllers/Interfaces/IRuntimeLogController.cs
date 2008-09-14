using System;
using Gallio.Icarus.Controllers.EventArgs;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IRuntimeLogController
    {
        event EventHandler<RuntimeLogEventArgs> LogMessage;
    }
}
