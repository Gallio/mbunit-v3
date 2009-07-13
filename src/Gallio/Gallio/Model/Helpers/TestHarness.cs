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
using Gallio.Common;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A test harness configures the test environment during test exploration and execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test harness may be stateful because it is used for only one test run then disposed.
    /// </para>
    /// </remarks>
    public class TestHarness : IDisposable
    {
        /// <summary>
        /// Creates the test harness.
        /// </summary>
        public TestHarness()
        {
        }

        /// <summary>
        /// Sets up the test harness.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses should override this method
        /// to prepare the AppDomain for testing such as by redirecting the console streams
        /// to a logging aparatus.
        /// </para>
        /// </remarks>
        public virtual void SetUp()
        {
        }

        /// <summary>
        /// Tears down the test harness.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses should override this method
        /// to cleanup the AppDomain after testing and revert all actions performed by <see cref="SetUp" />.
        /// </para>
        /// </remarks>
        public virtual void TearDown()
        {
        }

        /// <summary>
        /// Runs an action within the test harness.
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public void Run(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            RunImpl(action);
        }


        /// <summary>
        /// Runs an action within the test harness.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation simply invokes the action.  Subclasses should override this method
        /// to configure the Thread properties around the action such as setting the thread
        /// synchronization context.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to run, not null.</param>
        protected virtual void RunImpl(Action action)
        {
            action();
        }

        /// <summary>
        /// Disposes the test harness.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the test harness.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}