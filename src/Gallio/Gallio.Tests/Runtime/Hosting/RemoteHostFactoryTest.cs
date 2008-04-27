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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Gallio.Collections;
using Gallio.Framework.Utilities;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    public abstract class RemoteHostFactoryTest : AbstractHostFactoryTest
    {
        [Test]
        public void IsLocalFlagShouldBeFalse()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
                Assert.IsFalse(host.IsLocal);
        }

        [Test]
        public void DoCallbackHasRemoteSideEffects()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);

                Assert.AreEqual(0, callbackCounter);

                host.GetHostService().DoCallback(DoCallbackHasRemoteSideEffectsCallback);
                InterimAssert.Throws<Exception>(delegate { host.GetHostService().DoCallback(DoCallbackHasRemoteSideEffectsCallback); });
                host.GetHostService().DoCallback(DoCallbackHasRemoteSideEffectsCallback);

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
        public void HostRunsWithShadowCopyingEnabledOnRequest()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ShadowCopy = true;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.GetHostService().DoCallback(HostRunsWithShadowCopyingEnabledOnRequestCallback);
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
                host.GetHostService().DoCallback(HostRunsWithSpecifiedApplicationBaseDirectoryCallback);
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
                host.GetHostService().DoCallback(HostRunsWithSpecifiedConfigurationXmlCallback);
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
                host.GetHostService().DoCallback(HostRunsWithAssertUiEnabledCallback);
            }

            hostSetup.Configuration.AssertUiEnabled = false;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.GetHostService().DoCallback(HostRunsWithAssertUiDisabledCallback);
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

        public class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
