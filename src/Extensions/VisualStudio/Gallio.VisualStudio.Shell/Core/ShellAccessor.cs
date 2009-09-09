using System;
using Gallio.Runtime;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides easy access to the Shell.
    /// </summary>
    public static class ShellAccessor
    {
        /// <summary>
        /// Gets the Shell instance.
        /// </summary>
        public static DefaultShell Instance
        {
            get { return (DefaultShell) RuntimeAccessor.ServiceLocator.Resolve<IShell>(); }
        }
    }
}
