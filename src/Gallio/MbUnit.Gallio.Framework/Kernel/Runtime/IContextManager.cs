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
using System.Text;
using System.Threading;

namespace MbUnit.Framework.Kernel.Runtime
{
    /// <summary>
    /// The context manager tracks the <see cref="Context" /> associated with the current thread.
    /// </summary>
    /// <remarks>
    /// All context manager operations are thread safe.
    /// </remarks>
    public interface IContextManager
    {
        /// <summary>
        /// Gets the context of the current thread, or null if there is no
        /// current context.
        /// </summary>
        Context CurrentContext { get; }

        /// <summary>
        /// Gets the root context of the environment, or null if there is no
        /// root context.
        /// </summary>
        Context RootContext { get; }

        /// <summary>
        /// <para>
        /// Sets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="RootContext" /> unless the thread's
        /// default context has been overridden with <see cref="SetThreadDefaultContext" />.
        /// </para>
        /// <para>
        /// Changing the default context of a thread is useful for capturing existing threads created
        /// outside of a test into a particular context.  Among other things, this ensures that side-effects
        /// of the thread, such as writing text to the console, are recorded as part of the step
        /// represented by the specified context.
        /// </para>
        /// </remarks>
        /// <param name="thread">The thread</param>
        /// <param name="context">The context to associate with the thread, or null to reset the
        /// thread's default context to the root context</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        void SetThreadDefaultContext(Thread thread, Context context);

        /// <summary>
        /// <para>
        /// Gets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="RootContext" /> unless the thread's
        /// default context has been overridden with <see cref="SetThreadDefaultContext" />.
        /// </para>
        /// <para>
        /// Changing the default context of a thread is useful for capturing existing threads created
        /// outside of a test into a particular context.  Among other things, this ensures that side-effects
        /// of the thread, such as writing text to the console, are recorded as part of the step
        /// represented by the specified context.
        /// </para>
        /// </remarks>
        /// <param name="thread">The thread</param>
        /// <returns>The default context</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        Context GetThreadDefaultContext(Thread thread);
    }
}
