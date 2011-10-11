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
    /// An abstract base implementation of a test isolation context.
    /// </summary>
    public abstract class BaseTestIsolationContext : ITestIsolationContext
    {
        /// <summary>
        /// Disposes the test isolation context.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual bool RequiresSingleThreadedExecution
        {
            get { return false; }
        }

    	/// <summary>
    	/// Gets the test isolation options.
    	/// </summary>
		public TestIsolationOptions TestIsolationOptions { get; set; }

        /// <inheritdoc />
        public IDisposable BeginBatch(StatusReporter statusReporter)
        {
            if (statusReporter == null)
                throw new ArgumentNullException("statusReporter");

            return BeginBatchImpl(statusReporter);
        }

        /// <inheritdoc />
        public object RunIsolatedTask<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
            where TIsolatedTask : IsolatedTask, new()
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");
            if (statusReporter == null)
                throw new ArgumentNullException("statusReporter");

            return RunIsolatedTaskImpl<TIsolatedTask>(hostSetup, statusReporter, args);
        }

        /// <summary>
        /// Runs an isolated task within the test isolation context.
        /// </summary>
        /// <typeparam name="TIsolatedTask">The isolated task subclass.</typeparam>
        /// <param name="hostSetup">The host setup parameters for the isolated task, not null.</param>
        /// <param name="statusReporter">The status reporter, not null.</param>
        /// <param name="args">The task arguments, or null if none.  Must be serializable.</param>
        /// <returns>The task result, or null if none.  Must be serializable.</returns>
        protected abstract object RunIsolatedTaskImpl<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
            where TIsolatedTask : IsolatedTask, new();

        /// <summary>
        /// Begins a batch of isolated tasks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing and returns null.
        /// </para>
        /// </remarks>
        /// <param name="statusReporter">The status reporter, not null.</param>
        /// <returns>Returns an object that when disposed causes the batch to be terminated, or null if none.</returns>
        protected virtual IDisposable BeginBatchImpl(StatusReporter statusReporter)
        {
            return null;
        }

        /// <summary>
        /// Disposes the test isolation context.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}