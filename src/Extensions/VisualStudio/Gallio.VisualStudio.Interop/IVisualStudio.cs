using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides control over Visual Studio.
    /// </summary>
    public interface IVisualStudio
    {
        /// <summary>
        /// Gets the version of Visual Studio represented by this object.
        /// </summary>
        VisualStudioVersion Version { get; }

        /// <summary>
        /// Runs a block of code with the Visual Studio DTE.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The action runs in an STA thread context with appropriate <see cref="ComRetryMessageFilter" /> installed
        /// to guard against COM timeouts.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="VisualStudioException">Thrown if the call into Visual Studio failed</exception>
        void Call(Action<DTE> action);

        /// <summary>
        /// Makes Visual Studio the foreground window.
        /// </summary>
        void BringToFront();
    }
}
