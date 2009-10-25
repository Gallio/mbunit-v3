using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Specifies the integrity level of a Windows process.
    /// </summary>
    public enum ProcessIntegrityLevel
    {
        /// <summary>
        /// Unknown integrity level.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Process is running with Untrusted integrity.
        /// </summary>
        Untrusted,

        /// <summary>
        /// Process is running with Low integrity.
        /// </summary>
        Low,

        /// <summary>
        /// Process is running with Medium integrity.
        /// </summary>
        Medium,

        /// <summary>
        /// Process is running with High integrity.
        /// </summary>
        High,

        /// <summary>
        /// Process is running with System integrity.
        /// </summary>
        System,

        /// <summary>
        /// Process is running with Protected Process integrity.
        /// </summary>
        ProtectedProcess
    }
}
