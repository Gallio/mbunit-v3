using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// A tool window pane provides the chrome for a tool window.
    /// </summary>
    public interface IToolWindowPane
    {
        /// <summary>
        /// Gets or sets the window caption.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        string Caption { get; set; }

        /// <summary>
        /// Closes the tool window.
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the tool window.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the tool window.
        /// </summary>
        void Hide();
    }
}
