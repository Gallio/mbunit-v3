// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Hosting;
using Gallio.Model;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// <para>
    /// A host test domain contribution interacts with a <see cref="HostTestDomain" />
    /// to alter how the <see cref="IHost" /> is configured just prior to loading
    /// a <see cref="TestPackage"/>.
    /// </para>
    /// <para>
    /// An example of a contribution is one that registers additional
    /// bootstrap assemblies with binding redirects for use inside the host.
    /// </para>
    /// </summary>
    public interface IHostTestDomainContributor
    {
        /// <summary>
        /// Applies contributions to the <see cref="HostSetup" />
        /// prior to the creation of an <see cref="IHost" /> instance
        /// just prior to loading a <see cref="TestPackageConfig"/>.
        /// </summary>
        /// <param name="hostSetup">The host setup, never null</param>
        /// <param name="packageConfig">The test package configuration, never null</param>
        void ConfigureHost(HostSetup hostSetup, TestPackageConfig packageConfig);
    }
}