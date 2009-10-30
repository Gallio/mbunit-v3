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
using System.Collections.ObjectModel;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner
{
    /// <summary>
    /// The default implementation of <see cref="ITestRunnerManager" />.
    /// </summary>
    public class DefaultTestRunnerManager : ITestRunnerManager
    {
        private readonly IList<ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits>> factoryHandles;

        /// <summary>
        /// Creates a test runner manager.
        /// </summary>
        /// <param name="factoryHandles">The factory handles.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryHandles"/> is null.</exception>
        public DefaultTestRunnerManager(ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits>[] factoryHandles)
        {
            if (factoryHandles == null)
                throw new ArgumentNullException("factoryHandles");

            this.factoryHandles = factoryHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits>> TestRunnerFactoryHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits>>(factoryHandles); }
        }

        /// <inheritdoc />
        public ITestRunnerFactory GetFactory(string factoryName)
        {
            if (factoryName == null)
                throw new ArgumentNullException(@"factoryName");

            ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits> handle
                = GenericCollectionUtils.Find(factoryHandles, h => string.Compare(h.GetTraits().Name, factoryName, true) == 0);
            return handle != null ? handle.GetComponent() : null;
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner(string factoryName)
        {
            ITestRunnerFactory factory = GetFactory(factoryName);
            if (factory == null)
                throw new InvalidOperationException(String.Format("There is no test runner factory named '{0}'.", factoryName));

            return factory.CreateTestRunner();
        }
    }
}
