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
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Model;
using MbUnit.Model.Execution;
using MbUnit.Runner;

namespace MbUnit.Runner.Harness
{
    /// <summary>
    /// The test harness manages the lifecycle of test enumeration and execution.
    /// Contributors (such as test framework adapters) may attach event handlers
    /// to extend or modify the behavior at distinct points in the lifecycle.
    /// A new test harness instance is created when a test project is loaded into
    /// memory to serve as the ultimate container for all of its related resources.
    /// </summary>
    public interface ITestHarness : IDisposable
    {
        /// <summary>
        /// Gets the list of test assemblies loaded into the test harness.
        /// </summary>
        IList<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the event dispatcher.
        /// </summary>
        TestEventDispatcher EventDispatcher { get; }

        /// <summary>
        /// Gets the package loaded in the test harness, or null if none.
        /// </summary>
        TestPackage Package { get; }

        /// <summary>
        /// Gets the template tree builder for the test harness.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the template
        /// tree has not been built</exception>
        TemplateTreeBuilder TemplateTreeBuilder { get; }

        /// <summary>
        /// Gets the test tree builder for the test harness.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the test
        /// tree has not been built</exception>
        TestTreeBuilder TestTreeBuilder { get; }

        /// <summary>
        /// Adds a test framework.
        /// </summary>
        /// <param name="framework">The test framework to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="framework"/> is null</exception>
        void AddTestFramework(ITestFramework framework);

        /// <summary>
        /// Adds a test environment.
        /// </summary>
        /// <param name="environment">The test framework to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="environment"/> is null</exception>
        void AddTestEnvironment(ITestEnvironment environment);

        /// <summary>
        /// Adds a test assembly to the list.
        /// </summary>
        /// <param name="assembly">The assembly to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        void AddAssembly(Assembly assembly);

        /// <summary>
        /// Loads a test assembly and adds it to the list.
        /// </summary>
        /// <param name="assemblyFile">The filename of the assembly to load</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyFile"/> is null</exception>
        /// <exception cref="RunnerException">Thrown if the assembly could not be loaded</exception>
        Assembly LoadAssemblyFrom(string assemblyFile);

        /// <summary>
        /// Loads the test package into the test harness.
        /// </summary>
        /// <param name="package">The test package</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="package"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="LoadPackage" /> has already been called once
        /// because this interface does not support unloading packages except by disposing the harness
        /// and recreating it</exception>
        void LoadPackage(TestPackage package, IProgressMonitor progressMonitor);

        /// <summary>
        /// Populates the template tree.
        /// </summary>
        /// <param name="options">The template enumeration options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="LoadPackage" /> has not been called yet</exception>
        void BuildTemplates(TemplateEnumerationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Populates the test tree.
        /// </summary>
        /// <param name="options">The test enumeration options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BuildTemplates" /> has not been called yet</exception>
        void BuildTests(TestEnumerationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BuildTests" /> has not been called yet</exception>
        void RunTests(TestExecutionOptions options, IProgressMonitor progressMonitor);
    }
}