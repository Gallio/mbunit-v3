// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;
using Gallio.Runtime.Loader;

namespace Gallio.Model
{
    /// <summary>
    /// Default implementation of a test package explorer factory that aggregates over all registered frameworks.
    /// </summary>
    public class DefaultTestPackageExplorerFactory : ITestPackageExplorerFactory
    {
        private readonly ILoader loader;
        private readonly ITestFramework[] frameworks;

        /// <summary>
        /// Creates a factory.
        /// </summary>
        /// <param name="loader">The loader</param>
        /// <param name="frameworks">The frameworks to explore</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="loader"/> or <paramref name="frameworks"/> is null</exception>
        public DefaultTestPackageExplorerFactory(ILoader loader, ITestFramework[] frameworks)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");

            this.loader = loader;
            this.frameworks = frameworks;
        }

        /// <inheritdoc />
        public ITestExplorer CreateTestExplorer(TestPackageConfig testPackageConfig, IReflectionPolicy reflectionPolicy)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");

            TestPackage testPackage = new TestPackage(testPackageConfig, reflectionPolicy, loader);
            TestModel testModel = new TestModel(testPackage);

            AggregateTestExplorer aggregate = new AggregateTestExplorer(testModel);
            aggregate.AddExplorersForRequestedFrameworks(frameworks);

            return aggregate;
        }
    }
}
