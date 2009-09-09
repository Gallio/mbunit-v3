using System;
using System.Drawing;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// Describes how the command is presented in Visual Studio.
    /// </summary>
    public interface ICommandPresentation
    {
        /// <summary>
        /// Gets or sets the command icon.
        /// </summary>
        Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the command caption.
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Gets or sets the command tooltip.
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the command status.
        /// </summary>
        CommandStatus Status { get; set; }
    }
}
