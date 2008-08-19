// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Threading;
using Gallio.Concurrency;
using Gallio.Model;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
using Gallio.Utilities;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// A sandbox is an isolated environments for executing test actions.  It provides
    /// the ability to abort actions in progress so that the test runner can proceed
    /// to run other actions.
    /// </para>
    /// <para>
    /// Sandboxes are hierarchically structured.  When an outer sandbox is aborted, all
    /// of its inner sandboxes are likewise aborted.  A sandbox also provides the ability
    /// to create new child sandboxes at will so that test actions can be isolated with
    /// fine granularity.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This class is safe for use from multiple concurrent threads.
    /// </remarks>
    public sealed class Sandbox : IDisposable
    {
        private Sandbox parent;

        private readonly object syncRoot = new object();
        private List<ThreadAbortScope> scopes;
        private TestOutcome? abortOutcome;
        private string abortMessage;
        private event EventHandler aborted;
        private bool alreadyLoggedAbortOnce;

        /// <summary>
        /// Creates a root sandbox.
        /// </summary>
        public Sandbox()
        {
        }

        /// <summary>
        /// <para>
        /// An event that is dispatched when <see cref="Abort" /> is called.
        /// </para>
        /// <para>
        /// If the sandbox has already been aborted then the event handler is immediately invoked.
        /// </para>
        /// </summary>
        public event EventHandler Aborted
        {
            add
            {
                lock (syncRoot)
                {
                    if (!abortOutcome.HasValue)
                    {
                        aborted += value;
                        return;
                    }
                }

                EventHandlerUtils.SafeInvoke(value, this, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                    aborted -= value;
            }
        }

        /// <summary>
        /// Returns true if <see cref="Abort" /> was called.
        /// </summary>
        public bool WasAborted
        {
            get
            {
                lock (syncRoot)
                    return abortOutcome.HasValue;
            }
        }

        /// <summary>
        /// Returns the <see cref="TestOutcome" /> passed to the <see cref="Abort" />,
        /// or null if <see cref="Abort" /> has not been called.
        /// </summary>
        public TestOutcome? AbortOutcome
        {
            get
            {
                lock (syncRoot)
                    return abortOutcome;
            }
        }

        /// <summary>
        /// Gets a message that will be logged when the sandbox is aborted, or null if none.
        /// </summary>
        public string AbortMessage
        {
            get
            {
                lock (syncRoot)
                    return abortMessage;
            }
        }

        /// <summary>
        /// <para>
        /// Disposes the sandbox.
        /// </para>
        /// <para>
        /// All currently executing actions are aborted with <see cref="TestOutcome.Error" />
        /// if <see cref="Abort" /> has not already been called.
        /// </para>
        /// </summary>
        public void Dispose()
        {
            Abort(TestOutcome.Error, "The sandbox was disposed.");

            Sandbox cachedParent;
            lock (syncRoot)
            {
                cachedParent = parent;
                parent = null;
            }

            if (cachedParent != null)
                cachedParent.Aborted -= HandleParentAborted;
        }

        /// <summary>
        /// <para>
        /// Creates a child sandbox.
        /// </para>
        /// <para>
        /// When the parent sandbox is aborted, the child will likewise be aborted.  This policy
        /// offers a mechanism to scope actions recursively.
        /// </para>
        /// </summary>
        /// <returns>The child sandbox</returns>
        public Sandbox CreateChild()
        {
            Sandbox child = new Sandbox();
            child.parent = this;
            Aborted += child.HandleParentAborted;
            return child;
        }

        /// <summary>
        /// <para>
        /// Aborts all actions in progress within this context.
        /// </para>
        /// <para>
        /// The abort is persistent and cannot be reverted.  Therefore once aborted, no further
        /// test actions will be permitted to run.  Subsequent calls to <see cref="Abort" />
        /// will have no effect.
        /// </para>
        /// </summary>
        /// <param name="outcome">The outcome to be returned from aborted actions</param>
        /// <param name="message">A message to be logged when the action is aborted, or null if none</param>
        public void Abort(TestOutcome outcome, string message)
        {
            EventHandler cachedHandler;
            ThreadAbortScope[] cachedScopes;
            lock (syncRoot)
            {
                if (abortOutcome.HasValue)
                    return;

                abortOutcome = outcome;
                abortMessage = message;

                cachedScopes = scopes != null ? scopes.ToArray() : null;
                scopes = null;

                cachedHandler = aborted;
                aborted = null;
            }

            if (cachedScopes != null)
            {
                foreach (ThreadAbortScope scope in cachedScopes)
                    scope.Abort();
            }

            EventHandlerUtils.SafeInvoke(cachedHandler, this, EventArgs.Empty);
        }

        /// <summary>
        /// <para>
        /// Runs a test action.
        /// </para>
        /// <para>
        /// If the <see cref="Abort" /> method is called or has already been called, the action
        /// is aborted and the appropriate outcome is returned.  The abort is manifested as an
        /// asynchronous <see cref="ThreadAbortException" /> which should cause the action to
        /// terminate.  It may not terminate immediately, however.
        /// </para>
        /// <para>
        /// Produces an outcome in the following manner:
        /// <list type="bullet">
        /// <item>If the action completed without throwing an exception returns <see cref="TestOutcome.Passed"/>.</item>
        /// <item>If the action threw a <see cref="TestException" />, returns the value of the
        /// <see cref="TestException.Outcome" /> property.</item>
        /// <item>If the action threw an different kind of exception, logs
        /// the exception and returns <see cref="TestOutcome.Failed"/>.</item>
        /// <item>If the action was aborted, returns <see cref="AbortOutcome" />.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="description">A description of the action being performed,
        /// to be used as a log section name when reporting failures, or null if none</param>
        /// <returns>The outcome of the action</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public TestOutcome Run(Action action, string description)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ThreadAbortScope scope = null;
            try
            {
                lock (syncRoot)
                {
                    if (!abortOutcome.HasValue)
                    {
                        if (scopes == null)
                            scopes = new List<ThreadAbortScope>();

                        scope = new ThreadAbortScope();
                        scopes.Add(scope);
                    }
                }

                if (scope == null)
                    return HandleAbort(description, null);

                return RunWithScope(scope, action, description);
            }
            finally
            {
                if (scope != null)
                {
                    lock (syncRoot)
                        if (scopes != null)
                            scopes.Remove(scope);
                }
            }
        }

        private TestOutcome RunWithScope(ThreadAbortScope scope, Action action, string description)
        {
            try
            {
                ThreadAbortException ex = scope.Run(action);
                if (ex != null)
                    return HandleAbort(description, ex);

                return TestOutcome.Passed;
            }
            catch (Exception ex)
            {
                TestOutcome outcome;
                TestException testException = ex as TestException;
                if (testException != null)
                {
                    outcome = testException.Outcome;

                    if (testException.ExcludeStackTrace)
                        LogMessage(description, outcome, testException.HasNonDefaultMessage ? testException.Message : null, null);
                    else
                        LogMessage(description, outcome, null, testException);
                }
                else
                {
                    outcome = TestOutcome.Failed;
                    LogMessage(description, outcome, null, ex);
                }

                return outcome;
            }
        }

        private void HandleParentAborted(object sender, EventArgs e)
        {
            Sandbox parent = (Sandbox) sender;
            Abort(parent.AbortOutcome.Value, parent.AbortMessage);
        }

        private TestOutcome HandleAbort(string actionDescription, ThreadAbortException ex)
        {
            TestOutcome outcome = abortOutcome.Value;
            if (ex == null && alreadyLoggedAbortOnce)
                return outcome;

            alreadyLoggedAbortOnce = true;
            LogMessage(actionDescription, outcome, abortMessage, ex);
            return outcome;
        }

        private static void LogMessage(string actionDescription, TestOutcome outcome, string message, Exception ex)
        {
            if (string.IsNullOrEmpty(message) && ex == null)
                return;

            TestLogStreamWriter stream = GetLogStreamWriterForOutcome(outcome);
            using (actionDescription != null ? stream.BeginSection(actionDescription) : null)
            {
                if (! string.IsNullOrEmpty(message))
                    stream.WriteLine(message);

                if (ex != null)
                    stream.WriteException(StackTraceFilter.FilterException(ex));
            }
        }

        private static TestLogStreamWriter GetLogStreamWriterForOutcome(TestOutcome outcome)
        {
            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return TestLog.Default;
                case TestStatus.Failed:
                    return TestLog.Failures;
                default:
                    return TestLog.Warnings;
            }
        }
    }
}
