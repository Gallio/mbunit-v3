using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Specifies the installer operation.
    /// </summary>
    public enum InstallerOperation
    {
        /// <summary>
        /// Runs the install.
        /// </summary>
        Install,

        /// <summary>
        /// Runs the uninstall.
        /// </summary>
        Uninstall
    }
}
