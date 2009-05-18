using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// Provides services for managing utility commands.
    /// </summary>
    public interface IUtilityCommandManager
    {
        /// <summary>
        /// Gets handles for all registered utility commands.
        /// </summary>
        IList<ComponentHandle<IUtilityCommand, UtilityCommandTraits>> CommandHandles { get; }

        /// <summary>
        /// Gets the utility command with the specified name ignoring case, or null if not registered.
        /// </summary>
        /// <param name="name">The command name</param>
        /// <returns>The command</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        IUtilityCommand GetCommand(string name);
    }
}
