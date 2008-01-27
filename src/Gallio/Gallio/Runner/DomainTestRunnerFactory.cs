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

using System;
using System.Collections.Specialized;
using Gallio.Runner.Domains;

namespace Gallio.Runner
{
    /// <summary>
    /// A <see cref="ITestRunnerFactory" /> for <see cref="DomainTestRunner" /> using
    /// different implementations of <see cref="ITestDomainFactory" />.
    /// </summary>
    public class DomainTestRunnerFactory : ITestRunnerFactory
    {
        private readonly ITestDomainFactory domainFactory;
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates a test runner factory.
        /// </summary>
        /// <param name="domainFactory">The test domain factory</param>
        /// <param name="name">The test runner factory name</param>
        /// <param name="description">The test runner factory description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainFactory"/>,
        /// <paramref name="name"/> or <paramref name="description"/> is null</exception>
        public DomainTestRunnerFactory(ITestDomainFactory domainFactory, string name, string description)
        {
            if (domainFactory == null)
                throw new ArgumentNullException("domainFactory");
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.domainFactory = domainFactory;
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
        public ITestRunner CreateTestRunner(NameValueCollection runnerOptions)
        {
            return new DomainTestRunner(domainFactory);
        }
    }
}
