using System;
using Gallio.Runtime.Extensibility;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// A command describes an action that is presented in the Visual Studio command bar.
    /// </summary>
    [Traits(typeof(CommandTraits))]
    public interface ICommand
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="presentation">The command presentation, not null.</param>
        void Execute(ICommandPresentation presentation);

        /// <summary>
        /// Provides an opportunity for the command to update the status, text and other
        /// properties of its presentation.
        /// </summary>
        /// <param name="presentation">The command presentation, not null.</param>
        void Update(ICommandPresentation presentation);
    }
}
