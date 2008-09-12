using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Interface for objects that hold a reference to the shell.
    /// </summary>
    public interface IShellAccessor
    {
        /// <summary>
        /// Gets the shell.
        /// </summary>
        IShell Shell { get; }
    }
}
