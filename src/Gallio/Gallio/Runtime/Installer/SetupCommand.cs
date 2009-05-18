using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.UtilityCommands;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Installs or uninstalls components using the <see cref="IInstallerManager"/>.
    /// </summary>
    public class SetupCommand : BaseUtilityCommand<SetupCommand.Arguments>
    {
        private readonly IInstallerManager installerManager;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="installerManager">The installer manager, not null</param>
        public SetupCommand(IInstallerManager installerManager)
        {
            this.installerManager = installerManager;
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, Arguments arguments)
        {
            context.ProgressMonitorProvider.Run(progressMonitor =>
            {
                if (arguments.Install)
                {
                    installerManager.Install(context.Logger, progressMonitor);
                }
                else
                {
                    installerManager.Uninstall(context.Logger, progressMonitor);
                }
            });

            return 0;
        }

        /// <inheritdoc />
        public override bool ValidateArguments(Arguments arguments, CommandLineErrorReporter errorReporter)
        {
            if (!arguments.Install && !arguments.Uninstall
                || arguments.Install && arguments.Uninstall)
            {
                errorReporter("Exactly one of the options /install or /uninstall must be specified.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// When set to true, installs components.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "Installs all registered components.",
                LongName = "install",
                ShortName = "i")]
            public bool Install;

            /// <summary>
            /// When set to true, uninstalls components.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "Uninstalls all registered components.",
                LongName = "uninstall",
                ShortName = "u")]
            public bool Uninstall;
        }
    }
}
