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
using Gallio.Runner;
using Gallio.Model.Execution;
using Gallio.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Domains
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
    /// the host application with multiple frameworks.  The test domain API is
    /// designed to limit the number of round-trips required.  Moreover, all objects
    /// that pass through the API are serializable by value or can be marshalled
    /// using simple proxies.  This design facilitates remote operation and cross-version
    /// compatibility.
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
        /// <exception cref="RunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestPackageData TestPackageData { get; }

        /// <summary>
        /// Gets the test model, or null if tests have not been built yet.
        /// </summary>
        /// <exception cref="RunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestModelData TestModelData { get; }

        /// <summary>
        /// Sets the test listener for the domain.
        /// </summary>
        /// <param name="listener">The listener</param>
        void SetTestListener(ITestListener listener);

        /// <summary>
        /// Loads a test package into the test domain.
        /// </summary>
        /// <param name="packageConfig">The test package configuration to load</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="packageConfig"/> is null</exception>
        /// <exception cref="RunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void LoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Populates the test model.
        /// </summary>
        /// <param name="options">The test enumeration options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="RunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="LoadTestPackage" /> has not been called.</exception>
        void BuildTestModel(TestEnumerationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="options">The test execution options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BuildTestModel" /> has not been called.</exception>
        void RunTests(IProgressMonitor progressMonitor, TestExecutionOptions options);

        /// <summary>
        /// Unloads the current test package so that the test domain can
        /// be recycled for use with a different test package.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="RunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void UnloadPackage(IProgressMonitor progressMonitor);
    }
}