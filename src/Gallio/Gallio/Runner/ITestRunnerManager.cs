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
using Gallio.Runtime;

namespace Gallio.Runner
{
    /// <summary>
    /// A test runner manager enumerates the names of the
    /// <see cref="ITestRunnerFactory" /> services that are available and
    /// provides a mechanism for creating <see cref="ITestRunner" /> instances.
    /// </summary>
    public interface ITestRunnerManager
    {
        /// <summary>
        /// Gets a resolver for resolving registered
        /// <see cref="ITestRunnerFactory" /> components by name.
        /// </summary>
        IRegisteredComponentResolver<ITestRunnerFactory> FactoryResolver { get; }

        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="factoryName">The name of the test runner factory, matched case-insensitively</param>
        /// <returns>The test runner</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryName"/> is null</exception>
        ITestRunner CreateTestRunner(string factoryName);
    }
}
