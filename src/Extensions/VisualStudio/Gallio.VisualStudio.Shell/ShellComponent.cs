using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Abstract base class for a component that requires a reference to the <see cref="IShell" />.
    /// </summary>
    public abstract class ShellComponent : IShellAccessor
    {
        private readonly IShell shell;

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="shell">The associated shell</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shell"/> is null</exception>
        public ShellComponent(IShell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");

            this.shell = shell;
        }

        /// <inheritdoc />
        public IShell Shell
        {
            get { return shell; }
        }
    }
}
