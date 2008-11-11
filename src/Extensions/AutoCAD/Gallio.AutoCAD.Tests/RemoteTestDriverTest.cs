using System;
using System.Threading;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof(RemoteTestDriver))]
    public class RemoteTestDriverTest : BaseTestWithMocks
    {
        [Test]
        public void ShutdownIsCalledIfPingNotRecieved()
        {
            var timeout = TimeSpan.FromMilliseconds(200);
            var driver = new RemoteTestDriverTestDouble(timeout);
            Thread.Sleep(TimeSpan.FromMilliseconds(timeout.TotalMilliseconds * 1.5));
            Assert.IsTrue(driver.ShutdownCalled);
        }

        [Test]
        public void ShutdownIsNotCalledIfPingIsRecieved()
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(200);
            var driver = new RemoteTestDriverTestDouble(timeout);

            int numPings = 3;

            using (driver)
            {
                for (int i = 0; i < numPings; i++)
                {
                    driver.Ping();
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeout.TotalMilliseconds / 2));
                    Assert.IsFalse(driver.ShutdownCalled);
                }
            }
        }

        public class RemoteTestDriverTestDouble : RemoteTestDriver
        {
            public bool ShutdownCalled;

            public RemoteTestDriverTestDouble(TimeSpan pingTimeout)
                : base(pingTimeout)
            {
            }

            public override void Shutdown()
            {
                ShutdownCalled = true;
                base.Shutdown();
            }

            protected override void LoadImpl(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
            {
                throw new NotImplementedException();
            }

            protected override TestModelData ExploreImpl(TestExplorationOptions options, IProgressMonitor progressMonitor)
            {
                throw new NotImplementedException();
            }

            protected override void RunImpl(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
            {
                throw new NotImplementedException();
            }

            protected override void UnloadImpl(IProgressMonitor progressMonitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
