using System;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Abstract base class for shell extensions.
    /// </summary>
    public abstract class BaseShellExtension : IShellExtension, IShellAccessor
    {
        private IShell shell;

        /// <summary>
        /// Gets the shell, or null if the extension has been shut down.
        /// </summary>
        public IShell Shell
        {
            get { return shell; }
        }

        /// <inheritdoc />
        public void Initialize(IShell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");
            if (this.shell != null)
                throw new InvalidOperationException("The shell extension has already been initialized.");

            this.shell = shell;
            InitializeImpl();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            if (shell != null)
            {
                ShutdownImpl();
                shell = null;
            }
        }

        /// <summary>
        /// Initializes the shell extension.
        /// </summary>
        protected virtual void InitializeImpl()
        {
        }

        /// <summary>
        /// Shuts down the shell extension.
        /// </summary>
        protected virtual void ShutdownImpl()
        {
        }
    }
}
