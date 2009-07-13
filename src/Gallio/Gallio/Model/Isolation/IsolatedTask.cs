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
using Gallio.Common.Remoting;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// An isolated task runs in a <see cref="ITestIsolationContext" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses must be implemented carefully since they may run in a remote environment
    /// separate from the test runner.
    /// </para>
    /// </remarks>
    public abstract class IsolatedTask : LongLivedMarshalByRefObject, IDisposable
    {
        /// <summary>
        /// Disposes the task.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Runs the task.
        /// </summary>
        public abstract object Run(object[] args);

        /// <summary>
        /// Disposes the task.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly.</param>
        public virtual void Dispose(bool disposing)
        {
        }
    }
}