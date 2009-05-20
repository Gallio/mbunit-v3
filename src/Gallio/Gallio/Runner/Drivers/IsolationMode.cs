using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// Specifies the isolation mode for <see cref="HostedTestDriver" />.
    /// </summary>
    public enum IsolationMode
    {
        /// <summary>
        /// All test assemblies run within the same host in the same AppDomain.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this mode is used, any features that depend on test assembly
        /// configuration will not work.  For example, AppSettings will not be available.
        /// </para>
        /// </remarks>
        None,

        /// <summary>
        /// Each test assembly runs within the same host in different AppDomains.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this mode is used, all assemblies will run within the same host
        /// (implies they run within the same process).  This mode is efficient because
        /// there is less overhead starting up and tearing down hosts.  However, for this
        /// to work all test assemblies must target the same platform and .Net runtime
        /// version.
        /// </para>
        /// </remarks>
        AppDomainPerAssembly,

        /// <summary>
        /// Each test assembly runs within its own host.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this mode is used, all assemblies will run with their own host (possibly
        /// inside their own process, depending on the host type).  This mode enables
        /// a higher degree of isolation between test assemblies and has fewer constraints
        /// than other modes.
        /// </para>
        /// </remarks>
        HostPerAssembly,
    }
}
