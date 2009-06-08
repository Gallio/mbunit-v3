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
using System.Threading;

namespace Gallio.Framework
{
    /// <summary>
    /// A context cookie is used to unwind the context stack of the current thread
    /// to its previous state prior to a context having been entered.
    /// </summary>
    public struct TestContextCookie : IDisposable
    {
        private IDisposable inner;

        /// <summary>
        /// Creates a context cookie with a given dispose action.
        /// </summary>
        /// <param name="inner">The inner disposable object.</param>
        internal TestContextCookie(IDisposable inner)
        {
            this.inner = inner;
        }

        /// <summary>
        /// Exits the context that was entered when the cookie was granted.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Equivalent to calling <see cref="ExitContext" />.  This method is provded as a
        /// convenience for use with the C# using statement.
        /// </para>
        /// <para>
        /// This method will also exit nested contexts on the current thread that were
        /// entered after the context for which this cookie was produced but have not
        /// yet been exited.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the cookie belongs to a different
        /// <see cref="Thread" /> or if the context was already exited.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the context manager has been disposed.</exception>
        public void Dispose()
        {
            ExitContext();
        }

        /// <summary>
        /// Exits the context that was entered when the cookie was granted.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will also exit nested contexts on the current thread that were
        /// entered after the context for which this cookie was produced but have not
        /// yet been exited.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the cookie belongs to a different
        /// <see cref="Thread" /> or if the context was already exited.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the context manager has been disposed.</exception>
        public void ExitContext()
        {
            if (inner != null)
            {
                inner.Dispose();
                inner = null;
            }
        }
    }
}