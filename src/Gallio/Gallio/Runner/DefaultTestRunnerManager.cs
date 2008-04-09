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
using System.Collections.Specialized;
using Gallio.Runtime;

namespace Gallio.Runner
{
    /// <summary>
    /// The default implementation of <see cref="ITestRunnerManager" />.
    /// </summary>
    public class DefaultTestRunnerManager : ITestRunnerManager
    {
        private readonly IRegisteredComponentResolver<ITestRunnerFactory> factoryResolver;

        /// <summary>
        /// Creates a test runner manager.
        /// </summary>
        /// <param name="factoryResolver">The formatter resolver</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryResolver"/> is null</exception>
        public DefaultTestRunnerManager(IRegisteredComponentResolver<ITestRunnerFactory> factoryResolver)
        {
            if (factoryResolver == null)
                throw new ArgumentNullException("factoryResolver");

            this.factoryResolver = factoryResolver;
        }

        /// <inheritdoc />
        public IRegisteredComponentResolver<ITestRunnerFactory> FactoryResolver
        {
            get { return factoryResolver; }
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner(string factoryName, NameValueCollection runnerOptions)
        {
            if (factoryName == null)
                throw new ArgumentNullException(@"factoryName");
            if (runnerOptions == null)
                throw new ArgumentNullException(@"runnerOptions");

            ITestRunnerFactory factory = FactoryResolver.Resolve(factoryName);
            if (factory == null)
                throw new InvalidOperationException(String.Format("There is no test runner factory named '{0}'.", factoryName));

            return factory.CreateTestRunner(runnerOptions);
        }
    }
}
