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
using System.Threading;
using Gallio.Contexts;

namespace Gallio.Contexts
{
    /// <summary>
    /// <para>
    /// The context manager tracks the <see cref="Context" /> associated with threads.
    /// </para>
    /// </summary>
    /// <remarks>
    /// All context manager operations are thread safe.
    /// </remarks>
    public interface IContextManager
    {
        /// <summary>
        /// <para>
        /// Gets the context of the current thread, or null if there is no
        /// current context.
        /// </para>
        /// <para>
        /// A thread's current context is determined in the following manner:
        /// <list type="bullet">
        /// <item>If the thread's context stack is not empty then the top-most item of the
        /// stack is used.  <see cref="EnterContext" /> pushed a new item on this stack.</item>
        /// <item>Otherwise, if the thread has an default context, then it is used.
        /// <see cref="SetThreadDefaultContext" /> sets the default context for a thread.</item>
        /// <item>Otherwise, the <see cref="GlobalContext" /> is used.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Context information may flow across threads by inheritance such that a child
        /// thread acquires the context of its parent.  However, each thread has its own
        /// context stack distinct from that of any other.
        /// </para>
        /// </summary>
        Context CurrentContext { get; }

        /// <summary>
        /// Gets or the global context of the environment, or null if there is no
        /// such context.
        /// </summary>
        Context GlobalContext { get; set; }

        /// <summary>
        /// <para>
        /// Enters a context.
        /// </para>
        /// </summary>
        /// <param name="context">The context to enter, or null to enter a scope without a context</param>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value</returns>
        ContextCookie EnterContext(Context context);

        /// <summary>
        /// <para>
        /// Sets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="GlobalContext" /> unless the thread's
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
        /// thread's default context to inherit the <see cref="GlobalContext" /> once again</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        void SetThreadDefaultContext(Thread thread, Context context);

        /// <summary>
        /// <para>
        /// Gets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="GlobalContext" /> unless the thread's
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
        /// <returns>The default context of the thread</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        Context GetThreadDefaultContext(Thread thread);
    }
}