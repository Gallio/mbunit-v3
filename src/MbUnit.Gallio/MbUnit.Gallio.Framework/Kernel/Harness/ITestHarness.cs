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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Framework.Kernel.Harness
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
        /// Gets the runtime service for the test harness.
        /// </summary>
        IRuntime Runtime { get; }

        /// <summary>
        /// Gets the assembly resolver manager for the test harness.
        /// </summary>
        IAssemblyResolverManager AssemblyResolverManager { get; }

        /// <summary>
        /// Gets the list of test assemblies loaded into the test harness.
        /// </summary>
        IList<Assembly> Assemblies { get; }

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
        /// The event fired when the test harness is initialized.
        /// </summary>
        /// <remarks>
        /// This event provides test harness contributions with an opportunity to perform
        /// processing just after a test project is loaded.  For example, they might quickly
        /// scan the assemblies to configure assembly resolution strategies or
        /// to configure the behavior of built-in services in other ways.
        /// </remarks>
        event TypedEventHandler<ITestHarness, EventArgs> Initialized;

        /// <summary>
        /// The event fired when the test harness is building templates.
        /// </summary>
        /// <remarks>
        /// This event provides test harness contributions with an opportunity to populate
        /// the template tree.
        /// </remarks>
        event TypedEventHandler<ITestHarness, EventArgs> BuildingTemplates;

        /// <summary>
        /// The event fired when the test harness is building tests.
        /// </summary>
        /// <remarks>
        /// This event provides test harness contributions with an opportunity to populate
        /// the test tree.
        /// </remarks>
        event TypedEventHandler<ITestHarness, EventArgs> BuildingTests;

        /// <summary>
        /// The event fired when the test harness about to be disposed.
        /// </summary>
        /// <remarks>
        /// This event provides test harness contributions with an opportunity to perform
        /// cleanup actions as a test project is being unloaded.
        /// </remarks>
        event TypedEventHandler<ITestHarness, EventArgs> Disposing;

        /// <summary>
        /// Adds a test harness contributor.
        /// </summary>
        /// <param name="contributor">The contributor to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contributor"/> is null</exception>
        void AddContributor(ITestHarnessContributor contributor);

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
        /// <exception cref="TestHarnessException">Thrown if the assembly could not be loaded</exception>
        Assembly LoadAssemblyFrom(string assemblyFile);

        /// <summary>
        /// Initializes the test harness after its contributions have been registered.
        /// Causes the <see cref="Initialized" /> event to fire.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Populates the template tree.
        /// Causes the <see cref="BuildingTemplates" /> event to fire.
        /// </summary>
        /// <param name="options">The template enumeration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null</exception>
        void BuildTemplates(TemplateEnumerationOptions options);

        /// <summary>
        /// Populates the test tree.
        /// </summary>
        /// <param name="options">The test enumeration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BuildTemplates" /> has not been called yet</exception>
        void BuildTests(TestEnumerationOptions options);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BuildTests" /> has not been called yet</exception>
        void RunTests(TestExecutionOptions options);
    }
}
