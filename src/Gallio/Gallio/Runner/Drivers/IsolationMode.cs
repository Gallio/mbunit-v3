// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
