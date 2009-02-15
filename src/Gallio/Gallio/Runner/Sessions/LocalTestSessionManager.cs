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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gallio.Collections;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// A test session manager that maintains test sessions and runs
    /// in local memory.
    /// </summary>
    public class LocalTestSessionManager : ITestSessionManager
    {
        private readonly Dictionary<Guid, ITestSession> sessions;
        private readonly ITestRunnerFactory runnerFactory;
        private readonly IProgressMonitorProvider progressMonitorProvider;
        private readonly IReportManager reportManager;

        /// <summary>
        /// Creates a test session manager.
        /// </summary>
        /// <param name="runnerFactory">The test runner factory</param>
        /// <param name="progressMonitorProvider">The progress monitor provider to use
        /// for reporting progress of long running operations</param>
        /// <param name="reportManager">The report manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runnerFactory"/>,
        /// <paramref name="progressMonitorProvider"/> or <paramref name="reportManager"/> is null</exception>
        public LocalTestSessionManager(ITestRunnerFactory runnerFactory,
            IProgressMonitorProvider progressMonitorProvider, IReportManager reportManager)
        {
            if (runnerFactory == null)
                throw new ArgumentNullException("runnerFactory");
            if (progressMonitorProvider == null)
                throw new ArgumentNullException("progressMonitorProvider");
            if (reportManager == null)
                throw new ArgumentNullException("reportManager");

            this.runnerFactory = runnerFactory;
            this.progressMonitorProvider = progressMonitorProvider;
            this.reportManager = reportManager;

            sessions = new Dictionary<Guid, ITestSession>();
        }

        /// <inheritdoc />
        public event EventHandler<TestSessionEventArgs> SessionOpened;

        /// <inheritdoc />
        public event EventHandler<TestSessionEventArgs> SessionClosed;

        /// <inheritdoc />
        public ITestSession OpenSession()
        {
            ITestSession session = CreateSession();

            lock (sessions)
                sessions.Add(session.Id, session);

            EventHandlerUtils.SafeInvoke(SessionOpened, this,
                new TestSessionEventArgs(session));
            return session;
        }

        /// <inheritdoc />
        public void CloseSession(ITestSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            LocalTestSession localSession = session as LocalTestSession;
            if (localSession == null || localSession.Manager != this)
                throw new InvalidOperationException("The session does not belong to this test session manager.");

            lock (sessions)
            {
                if (!sessions.Remove(localSession.Id))
                    return;
            }

            localSession.Close();
            EventHandlerUtils.SafeInvoke(SessionClosed, this,
                new TestSessionEventArgs(session));
        }

        /// <inheritdoc />
        public IList<ITestSession> GetSessions()
        {
            ITestSession[] sessionList;
            lock (sessions)
                sessionList = GenericUtils.ToArray(sessions.Values);

            return new ReadOnlyCollection<ITestSession>(sessionList);
        }

        private ITestSession CreateSession()
        {
            return new LocalTestSession(this, runnerFactory, progressMonitorProvider, reportManager);
        }
    }
}
