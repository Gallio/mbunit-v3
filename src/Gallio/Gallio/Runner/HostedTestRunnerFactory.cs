// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;

namespace Gallio.Runner
{
    /// <summary>
    /// A <see cref="ITestRunnerFactory" /> for <see cref="HostedTestRunner" /> using
    /// different implementations of <see cref="IHostFactory" />.
    /// </summary>
    public class HostedTestRunnerFactory : ITestRunnerFactory
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFramework[] frameworks;
        private readonly string installationPath;
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates a test runner factory.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworks">The test frameworks</param>
        /// <param name="runtime">The runtime</param>
        /// <param name="name">The test runner factory name</param>
        /// <param name="description">The test runner factory description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworks"/>, <paramref name="runtime"/>, <paramref name="name"/> or <paramref name="description"/> is null</exception>
        public HostedTestRunnerFactory(IHostFactory hostFactory, ITestFramework[] frameworks, IRuntime runtime, string name, string description)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.hostFactory = hostFactory;
            this.frameworks = frameworks;
            this.installationPath = runtime.GetRuntimeSetup().InstallationPath;
            this.name = name;
            this.description = description;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public string Description
        {
            get { return description; }
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner()
        {
            return new HostedTestRunner(hostFactory, frameworks, installationPath);
        }
    }
}
