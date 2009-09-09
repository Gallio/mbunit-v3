using System;
using Gallio.VisualStudio.Shell.Core;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// Registers Visual Studio hooks for the <see cref="DefaultCommandManager" />.
    /// </summary>
    public class DefaultCommandManagerShellExtension : BaseShellExtension
    {
        private readonly DefaultCommandManager commandManager;

        /// <summary>
        /// Initializes the command manager shell extension.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        public DefaultCommandManagerShellExtension(ICommandManager commandManager)
        {
            this.commandManager = (DefaultCommandManager) commandManager;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            commandManager.Initialize();
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            commandManager.Shutdown();
        }
    }
}
