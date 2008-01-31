// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A proxy test listener forwards communication to a <see cref="ITestListener" />
    /// until it is disposed.  Once disposed, it responds to all subsequent events it
    /// receives by throwing <see cref="ObjectDisposedException" /> under the assumption that
    /// no further communication should occur unless it is due to a programming error
    /// or timing problem.
    /// </summary>
    public class ProxyTestListener : ITestListener, IDisposable
    {
        private ITestListener target;

        /// <summary>
        /// Creates a proxy for the specified listener.
        /// </summary>
        /// <param name="target">The target listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null</exception>
        public ProxyTestListener(ITestListener target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        /// <summary>
        /// <para>
        /// Disconnects the proxy.
        /// </para>
        /// </summary>
        public void Dispose()
        {
            target = null;
        }

        /// <inheritdoc />
        public void NotifyLifecycleEvent(LifecycleEventArgs e)
        {
            GetTargetOrThrowIfDisposed().NotifyLifecycleEvent(e);
        }

        /// <inheritdoc />
        public void NotifyLogEvent(LogEventArgs e)
        {
            GetTargetOrThrowIfDisposed().NotifyLogEvent(e);
        }

        private ITestListener GetTargetOrThrowIfDisposed()
        {
            ITestListener cachedTarget = target;
            if (cachedTarget == null)
                throw new ObjectDisposedException(GetType().Name);

            return cachedTarget;
        }
    }
}
