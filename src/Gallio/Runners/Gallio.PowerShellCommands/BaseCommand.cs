using System.Management.Automation;
using Castle.Core.Logging;

namespace Gallio.PowerShellCommands
{
    /// <summary>
    /// Abstract base class for PowerShell commands.
    /// Provides some useful runtime support.
    /// </summary>
    public abstract class BaseCommand : PSCmdlet
    {
        private CmdletLogger logger;

        /// <summary>
        /// Gets the logger for the cmdlet.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (logger == null)
                    logger = new CmdletLogger(this);
                return logger;
            }
        }
    }
}
