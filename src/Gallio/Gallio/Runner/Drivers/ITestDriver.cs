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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Messages;
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
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <param name="testRunnerOptions">The test runner options</param>
        /// <param name="logger">The logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimeSetup"/>,
        /// <paramref name="testRunnerOptions"/>, or <paramref name="logger"/> is null</exception>
        void Initialize(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger);

        /// <summary>
        /// Explores tests in a test package.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <param name="testExplorationOptions">The test exploration options</param>
        /// <param name="testExplorationListener">The test exploration listener</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <returns>The test report</returns>
        void Explore(TestPackageConfig testPackageConfig,
            TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener,
            IProgressMonitor progressMonitor);

        /// <summary>
        /// Explores and runs tests in a test package.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <param name="testExplorationOptions">The test exploration options</param>
        /// <param name="testExplorationListener">The test exploration listener</param>
        /// <param name="testExecutionOptions">The test execution options</param>
        /// <param name="testExecutionListener">The test execution listener</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <returns>The test report</returns>
        void Run(TestPackageConfig testPackageConfig,
            TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener,
            TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener,
            IProgressMonitor progressMonitor);
    }
}
