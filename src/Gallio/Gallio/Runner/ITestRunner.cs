// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model.Execution;
using Gallio.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner
{
    /// <summary>
    /// A test runner provides operations for loading test projects, enumerating
    /// templates and tests, running tests, and generating reports.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used to simplify test runner integration
    /// concerns by gathering the entire lifecycle in one place and enabling
    /// it to be monitored by adding appropriate event handlers.  The creation
    /// and disposal of resources such as a test domain and harness are taken
    /// care of automatically by implementations of this interface so that the
    /// client code is decoupled from these internal lifecycle issues.
    /// </remarks>
    public interface ITestRunner : IDisposable
    {
        /// <summary>
        /// Event fired once <see cref="LoadTestPackage" /> completes.
        /// </summary>
        /// <remarks>
        /// This event will be fired even if the operation failed in which case
        /// <see cref="TestPackageData" /> will be null.
        /// </remarks>
        event EventHandler LoadTestPackageComplete;

        /// <summary>
        /// Event fired once <see cref="BuildTestModel" /> completes.
        /// </summary>
        /// <remarks>
        /// This event will be fired even if the operation failed in which case
        /// <see cref="TestModelData" /> will be null.
        /// </remarks>
        event EventHandler BuildTestModelComplete;

        /// <summary>
        /// Event fired before <see cref="RunTests" /> begins doing work.
        /// </summary>
        event EventHandler RunTestsStarting;

        /// <summary>
        /// Event fired once <see cref="RunTests" /> completes.
        /// </summary>
        event EventHandler RunTestsComplete;

        /// <summary>
        /// Gets the event dispatcher for the test runner.
        /// </summary>
        TestEventDispatcher EventDispatcher { get; }

        /// <summary>
        /// Gets or sets the test enumeration options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        TestEnumerationOptions TestEnumerationOptions { get; set; }

        /// <summary>
        /// Gets or sets the test execution options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        TestExecutionOptions TestExecutionOptions { get; set; }

        /// <summary>
        /// Gets the currently loaded test package, or null if none has been loaded yet.
        /// </summary>
        TestPackageData TestPackageData { get; }

        /// <summary>
        /// Gets the test model, or null if tests have not been built yet.
        /// </summary>
        TestModelData TestModelData { get; }

        /// <summary>
        /// Loads a test package.
        /// </summary>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="packageConfig"/> is null</exception>
        void LoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Builds the test tree using the current <see cref="TestEnumerationOptions" />.
        /// Populates <see cref="TestModelData" /> accordingly.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void BuildTestModel(IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void RunTests(IProgressMonitor progressMonitor);
    }
}