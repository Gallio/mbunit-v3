// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using Gallio.Runtime.Logging;
using Gallio.Collections;
using Gallio.Runtime;
using Gallio.Framework.Utilities;
using Gallio.Runtime.Hosting;
using Gallio.Reflection;
using Gallio.Runtime.Windsor;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    public abstract class AbstractHostFactoryTest : BaseUnitTest
    {
        public abstract IHostFactory Factory { get; }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            Factory.CreateHost(null, new LogStreamLogger());
        }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfLoggerIsNull()
        {
            Factory.CreateHost(new HostSetup(), null);
        }

        [Test]
        public void PingSucceedsUntilHostIsDisposed()
        {
            IHost host;
            using (host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                // Should work fine.
                host.Ping();
            }

            // Should fail.
            InterimAssert.Throws<Exception>(delegate { host.Ping(); });
        }

        [Test]
        public void CreateInstanceCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                Type remoteType = typeof(ArrayList);
                ObjectHandle handle = host.CreateInstance(remoteType.Assembly.FullName, remoteType.FullName);

                Assert.IsInstanceOfType(remoteType, handle.Unwrap());
            }
        }

        [Test]
        public void CreateInstanceFromCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                Type serviceType = typeof(TestService);
                TestService serviceProxy = (TestService)host.CreateInstanceFrom(
                    AssemblyUtils.GetAssemblyLocalPath(serviceType.Assembly), serviceType.FullName).Unwrap();

                Assert.AreEqual(42, serviceProxy.Add(23, 19));
            }
        }

        [Test]
        public void DoCallbackHasRemoteSideEffects()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);

                Assert.AreEqual(0, callbackCounter);

                host.DoCallback(DoCallbackHasRemoteSideEffectsCallback);
                InterimAssert.Throws<Exception>(delegate { host.DoCallback(DoCallbackHasRemoteSideEffectsCallback); });
                host.DoCallback(DoCallbackHasRemoteSideEffectsCallback);

                Assert.AreEqual(0, callbackCounter);
            }
        }

        private static int callbackCounter;
        private static void DoCallbackHasRemoteSideEffectsCallback()
        {
            if (callbackCounter++ == 1)
                throw new Exception("Test exception.");
        }

        [Test]
        public void RemoteRuntimeCanAccessTheLogger()
        {
            ILogger logger = Mocks.CreateMock<ILogger>();

            using (Mocks.Record())
            {
                logger.Log(LogSeverity.Debug, "message", null);
            }

            using (Mocks.Playback())
            {
                using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
                {
                    HostAssemblyResolverHook.Install(host);
                    host.InitializeRuntime(WindsorRuntimeFactory.Instance, new RuntimeSetup(), logger);

                    host.DoCallback(RemoteRuntimeCanAccessTheLoggerCallback);

                    host.ShutdownRuntime();
                }
            }
        }
        private static void RemoteRuntimeCanAccessTheLoggerCallback()
        {
            RuntimeAccessor.Logger.Log(LogSeverity.Debug, "message", null);
        }

        [Test]
        public void HostRunsInRequestedWorkingDirectory()
        {
            string oldWorkingDirectory = Environment.CurrentDirectory;

            HostSetup hostSetup = new HostSetup();
            hostSetup.WorkingDirectory = Path.GetTempPath();

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsInRequestedWorkingDirectoryCallback);
            }

            Assert.AreEqual(oldWorkingDirectory, Environment.CurrentDirectory,
                "Current working directory of the calling process should be unchanged or at least restored once the host is disposed.");
        }
        private static void HostRunsInRequestedWorkingDirectoryCallback()
        {
            AssertArePathsEqualIgnoringFinalBackslash(Path.GetTempPath(), Environment.CurrentDirectory);
        }

        [Test]
        public void HostRunsWithShadowCopyingEnabledOnRequest()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ShadowCopy = true;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsWithShadowCopyingEnabledOnRequestCallback);
            }
        }
        private static void HostRunsWithShadowCopyingEnabledOnRequestCallback()
        {
            Assert.IsTrue(AppDomain.CurrentDomain.ShadowCopyFiles);
        }

        [Test]
        public void HostRunsWithSpecifiedApplicationBaseDirectory()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ApplicationBaseDirectory = Path.GetTempPath();

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsWithSpecifiedApplicationBaseDirectoryCallback);
            }
        }
        private static void HostRunsWithSpecifiedApplicationBaseDirectoryCallback()
        {
            AssertArePathsEqualIgnoringFinalBackslash(Path.GetTempPath(), AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void HostRunsWithSpecifiedConfigurationXml()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.Configuration.ConfigurationXml =
                  "<configuration>"
                + "  <appSettings>"
                + "    <add key=\"TestSetting\" value=\"TestValue\" />"
                + "  </appSettings>"
                + "</configuration>";

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsWithSpecifiedConfigurationXmlCallback);
            }
        }
        private static void HostRunsWithSpecifiedConfigurationXmlCallback()
        {
            Assert.AreEqual("TestValue", ConfigurationManager.AppSettings.Get("TestSetting"));
        }

        [Test]
        public void HostRunsWithSpecifiedAssertUiFlag()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.Configuration.AssertUiEnabled = true;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsWithAssertUiEnabledCallback);
            }

            hostSetup.Configuration.AssertUiEnabled = false;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(HostRunsWithAssertUiDisabledCallback);
            }
        }
        private static void HostRunsWithAssertUiEnabledCallback()
        {
            Assert.IsTrue(GetDefaultTraceListener().AssertUiEnabled);
        }
        private static void HostRunsWithAssertUiDisabledCallback()
        {
            Assert.IsFalse(GetDefaultTraceListener().AssertUiEnabled);
        }

        private static DefaultTraceListener GetDefaultTraceListener()
        {
            return (DefaultTraceListener) CollectionUtils.Find<TraceListener>(Debug.Listeners,
                delegate(TraceListener listener) { return listener is DefaultTraceListener; });
        }

        private static void AssertArePathsEqualIgnoringFinalBackslash(string expected, string actual)
        {
            Assert.AreEqual(AddFinalBackslashIfAbsent(expected), AddFinalBackslashIfAbsent(actual));
        }

        private static string AddFinalBackslashIfAbsent(string value)
        {
            return value.EndsWith("\\") ? value : value + '\\';
        }

        public class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
