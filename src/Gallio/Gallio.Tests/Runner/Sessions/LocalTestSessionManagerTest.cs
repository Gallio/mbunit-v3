using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Runner.Sessions;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Sessions
{
    [TestsOn(typeof(LocalTestSessionManager))]
    public class LocalTestSessionManagerTest
    {
        public class WhenSessionIsOpened : BaseUnitTest
        {
            [Test]
            public void TheNewSessionInitiallyHasNoTestRuns()
            {
                LocalTestSessionManager manager = new LocalTestSessionManager(
                    Mocks.Stub<ITestRunnerFactory>(), Mocks.Stub<IProgressMonitorProvider>(), Mocks.Stub<IReportManager>());

                ITestSession session = manager.OpenSession();

                Assert.IsNotNull(session, "The session should not be null.");
                Assert.IsNull(session.CurrentTestRun, "Session should have no current test run.");
            }

            [Test]
            public void SessionManagerFiresAnEvent()
            {
                ITestSession eventArgsSession = null;
                LocalTestSessionManager manager = new LocalTestSessionManager(
                    Mocks.Stub<ITestRunnerFactory>(), Mocks.Stub<IProgressMonitorProvider>(), Mocks.Stub<IReportManager>());

                manager.SessionOpened += (sender, e) =>
                {
                    Assert.AreSame(manager, sender, "Sender and manager should be same.");
                    eventArgsSession = e.Session;
                };

                ITestSession session = manager.OpenSession();
                Assert.AreSame(session, eventArgsSession, "The new session should have been passed in the event.");
            }
        }

        public class WhenSessionIsClosed : BaseUnitTest
        {
            [Test]
            public void SessionManagerFiresAnEvent()
            {
                ITestSession eventArgsSession = null;
                LocalTestSessionManager manager = new LocalTestSessionManager(
                    Mocks.Stub<ITestRunnerFactory>(), Mocks.Stub<IProgressMonitorProvider>(), Mocks.Stub<IReportManager>());
                ITestSession session = manager.OpenSession();

                manager.SessionClosed += (sender, e) =>
                {
                    Assert.AreSame(manager, sender, "Sender and manager should be same.");
                    eventArgsSession = e.Session;
                };
                manager.CloseSession(session);

                Assert.AreSame(session, eventArgsSession, "The new session should have been passed in the event.");
            }

            [Test]
            public void NothingHappensIfSessionIsAlreadyClosed()
            {
                LocalTestSessionManager manager = new LocalTestSessionManager(
                    Mocks.Stub<ITestRunnerFactory>(), Mocks.Stub<IProgressMonitorProvider>(), Mocks.Stub<IReportManager>());
                ITestSession session = manager.OpenSession();
                manager.CloseSession(session);

                bool closeCalled = false;
                manager.SessionClosed += (sender, e) => closeCalled = true;

                manager.CloseSession(session);
                Assert.IsFalse(closeCalled, "SessionClosed should not be called redundantly.");
            }

            [Test]
            public void CloseThrowsIfArgumentIsNull()
            {
                LocalTestSessionManager manager = new LocalTestSessionManager(
                    Mocks.Stub<ITestRunnerFactory>(), Mocks.Stub<IProgressMonitorProvider>(), Mocks.Stub<IReportManager>());
                Assert.Throws<ArgumentNullException>(() => manager.CloseSession(null));
            }
        }
    }
}
