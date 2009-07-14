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
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Model
{
    /// <summary>
    /// Provides services based on the installed set of test frameworks.
    /// </summary>
    public interface ITestFrameworkManager
    {
        /// <summary>
        /// Gets handles for all registered test frameworks.
        /// </summary>
        IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> FrameworkHandles { get; }

        /// <summary>
        /// Gets an aggregate test driver for selected frameworks.
        /// </summary>
        /// <param name="frameworkIdFilter">A predicate to select which frameworks should
        /// be consulted based on the framework id, or null to include all frameworks.</param>
        /// <param name="logger">The logger for the test driver.</param>
        /// <returns>The test driver.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        ITestDriver GetTestDriver(Predicate<string> frameworkIdFilter, ILogger logger);
    }
}
