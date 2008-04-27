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
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A test driver controls how tests are loaded, explored, and run.
    /// </summary>
    public interface ITestDriver : IDisposable
    {
        /// <summary>
        /// Initializes the test driver.
        /// </summary>
        /// <param name="runtimeFactory">The runtime factory</param>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <param name="logger">The logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimeFactory"/>,
        /// <paramref name="runtimeSetup"/> or <paramref name="logger"/> is null</exception>
        void Initialize(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger);

        /// <summary>
        /// Loads a test package.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        void Load(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Explores the tests.
        /// </summary>
        /// <param name="options">The test exploration options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <returns>The test model data</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        TestModelData Explore(TestExplorationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <param name="listener">The test listener</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/>,
        /// <paramref name="listener"/> or <paramref name="progressMonitor"/> is null</exception>
        void Run(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor);

        /// <summary>
        /// Unloads the tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void Unload(IProgressMonitor progressMonitor);
    }
}
