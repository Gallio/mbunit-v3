using System;

namespace Gallio.VisualStudio.Toolkit
{
    /// <summary>
    /// A component is an object that manages some resource on behalf of an add-in.
    /// </summary>
    public abstract class Component
    {
        private readonly Shell shell;

        /// <summary>
        /// Creates a component.
        /// </summary>
        /// <param name="shell">The shell that owns the component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shell"/> is null</exception>
        public Component(Shell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");

            this.shell = shell;
        }

        /// <summary>
        /// Gets the shell that owns the component.
        /// </summary>
        public Shell Shell
        {
            get { return shell; }
        }
    }
}
