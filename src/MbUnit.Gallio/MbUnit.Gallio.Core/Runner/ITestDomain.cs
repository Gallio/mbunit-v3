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
using MbUnit.Core.Serialization;

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
        /// Adds or removes a progress event handler to receive notifications
        /// of operations that are in progress.
        /// </summary>
        //event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Gets the currently loaded test project, or null if none.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestProjectInfo TestProject { get; }

        /// <summary>
        /// Gets the root of the test template tree, or null if the test templates have not
        /// been built yet.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestTemplateInfo TestTemplateTreeRoot { get; }

        /// <summary>
        /// Gets the root of the test tree, or null if the tests have not been built yet.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        TestInfo TestTreeRoot { get; }

        /// <summary>
        /// Loads a test project into the test domain.
        /// </summary>
        /// <param name="project">The test project to load</param>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void LoadProject(TestProjectInfo project);

        /// <summary>
        /// Builds the tree of test templates.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void BuildTestTemplates();

        /// <summary>
        /// Builds the tree of tests.
        /// </summary>
        void BuildTests();

        /// <summary>
        /// Runs the tests.
        /// </summary>
        void RunTests();

        /// <summary>
        /// Unloads the current test project so that the test domain can
        /// be recycled for use with a different test project.
        /// </summary>
        /// <exception cref="FatalRunnerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the domain has been disposed</exception>
        void UnloadProject();
    }
}
