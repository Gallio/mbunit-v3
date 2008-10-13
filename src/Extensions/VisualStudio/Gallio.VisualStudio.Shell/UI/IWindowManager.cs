using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// Manages Gallio tool windows and editors.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Finds the shell tool window with the specified window id.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <returns>The tool window, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        ShellToolWindow FindToolWindow(string id);

        /// <summary>
        /// Opens a tool window.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <param name="window">The tool window to open</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="window"/> is null</exception>
        void OpenToolWindow(string id, ShellToolWindow window);

        /// <summary>
        /// Closes the shell tool window with the specified window id.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        void CloseToolWindow(string id);
    }
}
