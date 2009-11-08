using Gallio.UI.DataBinding;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.UI.Menus
{
    /// <summary>
    /// Wraps an <see cref="ICommand">ICommand</see> and provides
    /// hints for the UI.
    /// </summary>
    /// <remarks>
    /// Inspired by the WPF Command pattern.
    /// http://msdn.microsoft.com/en-us/library/system.windows.input.icommand.aspx
    /// </remarks>
    public class MenuCommand
    {
        /// <summary>
        /// The command that will be executed.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Whether the command can currently be executed, or not.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public Observable<bool> CanExecute { get; set; }

        /// <summary>
        /// The text description of the command.
        /// </summary>
        public string Text { get; set; }

        // TODO: icons & shortcuts

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuCommand()
        {
            CanExecute = new Observable<bool>(true);
        }
    }
}
