using System;
using System.Drawing;
using Gallio.Runtime.Extensibility;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// Provides traits of a <see cref="ICommand" />.
    /// </summary>
    public class CommandTraits : Traits
    {
        private readonly string commandName;
        private readonly string[] commandBarPaths;

        /// <summary>
        /// Initializes command traits.
        /// </summary>
        /// <param name="commandName">The Visual Studio command name.</param>
        /// <param name="commandBarPaths">The array of Visual Studio command bar paths with command bar name segments delimited by '\'.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> or
        /// <paramref name="commandBarPaths"/> is null.</exception>
        public CommandTraits(string commandName, string[] commandBarPaths)
        {
            if (commandName == null)
                throw new ArgumentNullException("commandName");
            if (commandBarPaths == null)
                throw new ArgumentNullException("commandBarPaths");

            this.commandName = commandName;
            this.commandBarPaths = commandBarPaths;
        }

        /// <summary>
        /// Gets the Visual Studio command name.
        /// </summary>
        public string CommandName
        {
            get { return commandName; }
        }

        /// <summary>
        /// Gets the array of Visual Studio command bar paths with command bar name segments delimited by '\'.
        /// </summary>
        public string[] CommandBarPaths
        {
            get { return commandBarPaths; }
        }

        /// <summary>
        /// Gets or sets the command icon.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the command caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the command tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the command status.
        /// </summary>
        public CommandStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the command style.
        /// </summary>
        public CommandStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the command control type.
        /// </summary>
        public CommandControlType ControlType { get; set; }
    }
}
