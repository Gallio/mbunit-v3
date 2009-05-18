using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to verify that the plugin metadata and installation parameters are correct.
    /// </summary>
    public class VerifyInstallationCommand : BaseUtilityCommand<object>
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="runtime">The runtime, not null</param>
        public VerifyInstallationCommand(IRuntime runtime)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, object arguments)
        {
            return runtime.VerifyInstallation() ? 0 : 1;
        }
    }
}
