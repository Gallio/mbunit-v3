using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// The default utility command manager implementation.
    /// </summary>
    public class DefaultUtilityCommandManager : IUtilityCommandManager
    {
        private ComponentHandle<IUtilityCommand, UtilityCommandTraits>[] commandHandles;

        /// <summary>
        /// Creates a utility command manager.
        /// </summary>
        /// <param name="commandHandles">The command handles, not null</param>
        public DefaultUtilityCommandManager(ComponentHandle<IUtilityCommand, UtilityCommandTraits>[] commandHandles)
        {
            this.commandHandles = commandHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<IUtilityCommand, UtilityCommandTraits>> CommandHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<IUtilityCommand, UtilityCommandTraits>>(commandHandles); }
        }

        /// <inheritdoc />
        public IUtilityCommand GetCommand(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (var commandHandle in commandHandles)
                if (string.Compare(commandHandle.GetTraits().Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return commandHandle.GetComponent();

            return null;
        }
    }
}
