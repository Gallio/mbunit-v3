using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// Specifies the installation mode for the TDNet runner with respect to a given framework.
    /// </summary>
    public enum TDNetRunnerInstallationMode
    {
        /// <summary>
        /// The runner has been disabled for a particular framework.
        /// </summary>
        Disabled,

        /// <summary>
        /// The runner should be used in preference to any other runner for the framework is installed on the machine.
        /// </summary>
        Preferred,

        /// <summary>
        /// The runner should be used if no other runner for the framework is installed on the machine.
        /// </summary>
        Default
    }
}
