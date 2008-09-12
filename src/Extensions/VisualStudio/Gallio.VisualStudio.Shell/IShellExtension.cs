using System;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Extends the <see cref="IShell" /> with additional contributions.
    /// </summary>
    public interface IShellExtension
    {
        /// <summary>
        /// Initializes the shell extension.
        /// </summary>
        /// <param name="shell">The shell</param>
        void Initialize(IShell shell);

        /// <summary>
        /// Shuts down the shell extension.
        /// </summary>
        void Shutdown();
    }
}
