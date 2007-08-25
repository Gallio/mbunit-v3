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
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// <para>
    /// The test domain interface provides services for interacting with tests
    /// hosted in an isolated domain (presumably in its own AppDomain).  Where the
    /// domain is hosted is irrelvant.  It might be in an AppDomain within the same
    /// process or it might reside in another process altogether.
    /// </para>
    /// <para>
    /// Test domain implementations should be designed to permit interoperability of
    /// the host application with different versions of MbUnit linked to the code modules
    /// being tested.  For this reason, the test domain API only exposes objects that
    /// are serializable by value or interfaces for which proxies can be easily
    /// constructed with efficient interfaces that require few round-trips.
    /// </para>
    /// <para>
    /// Test domain implementations based on remote calls should implement the test
    /// domain as a proxy over the real remote interface rather than, for instance,
    /// subclassing <see cref="MarshalByRefObject" /> and supplying the application
    /// with a transparent proxy to be used directly.  The test domain implementation
    /// should protect the main application from configuration concerns and failure
    /// conditions resulting from the use of remoting internally.
    /// </para>
    /// <para>
    /// Calling <see cref="IDisposable.Dispose" /> should never throw an exception.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This interface is not thread-safe.
    /// </remarks>
    public interface ITestDomain : IDisposable
    {
        /// <summary>
        /// Gets the currently loaded test package, or null if none.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestPackage Package { get; }

        /// <summary>
        /// Gets the template model, or null if templates have not been built yet.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TemplateModel TemplateModel { get; }

        /// <summary>
        /// Gets the test model, or null if tests have not been built yet.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestModel TestModel { get; }

        /// <summary>
        /// Sets the event listener for the domain.
        /// </summary>
        /// <param name="listener">The listener</param>
        void SetEventListener(IEventListener listener);

        /// <summary>
        /// Loads a test package into the test domain.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="package">The test package to load</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="package"/> is null</exception>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void LoadPackage(IProgressMonitor progressMonitor, TestPackage package);

        /// <summary>
        /// Builds the tree of templates.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="options">The template enumeration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void BuildTemplates(IProgressMonitor progressMonitor, TemplateEnumerationOptions options);

        /// <summary>
        /// Builds the tree of tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="options">The test enumeration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        void BuildTests(IProgressMonitor progressMonitor, TestEnumerationOptions options);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="options">The test execution options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        void RunTests(IProgressMonitor progressMonitor, TestExecutionOptions options);

        /// <summary>
        /// Unloads the current test package so that the test domain can
        /// be recycled for use with a different test package.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void UnloadPackage(IProgressMonitor progressMonitor);
    }
}
