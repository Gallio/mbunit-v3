// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Hosting;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// A test isolation context is responsible for running test code in an isolated
    /// (possibly remote) environment.
    /// </summary>
    public interface ITestIsolationContext : IDisposable
    {
        /// <summary>
        /// Gets or sets whether the test isolation context requires test drivers to try to run test
        /// code within the same thread on which the isolated task is executed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A test isolation context may require single-threaded execution because it needs to
        /// ensure that tests run within the UI loop of an application or due to other
        /// synchornization concerns.
        /// </para>
        /// </remarks>
        bool RequiresSingleThreadedExecution { get; }

        /// <summary>
        /// Begins a batch of isolated tasks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The isolation context may optimize the execution of subsequent isolated tasks
        /// given that they are all part of the same batch.
        /// </para>
        /// </remarks>
        /// <param name="statusReporter">The status reporter.</param>
        /// <returns>Returns an object that when disposed causes the batch to be terminated, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="statusReporter"/> is null.</exception>
        IDisposable BeginBatch(StatusReporter statusReporter);

        /// <summary>
        /// Runs an isolated task within the test isolation context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Since the task may be executed remotely, make sure that all
        /// arguments and results are serializable.
        /// </para>
        /// </remarks>
        /// <typeparam name="TIsolatedTask">The isolated task subclass.</typeparam>
        /// <param name="hostSetup">The host setup parameters for the isolated task.</param>
        /// <param name="statusReporter">The status reporter.</param>
        /// <param name="args">The task arguments, or null if none.  Must be serializable.</param>
        /// <returns>The task result, or null if none.  Must be serializable.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="statusReporter"/> is null.</exception>
        object RunIsolatedTask<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
            where TIsolatedTask : IsolatedTask, new();
    }
}