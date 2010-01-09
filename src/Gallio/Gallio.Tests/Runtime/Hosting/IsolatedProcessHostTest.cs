// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Gallio.Common.Platform;
using Gallio.Framework;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Tests.Common.Remoting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    [TestsOn(typeof(IsolatedProcessHost))]
    [TestsOn(typeof(IsolatedProcessHostFactory))]
    [DependsOn(typeof(BaseHostFactoryTest))]
    [DependsOn(typeof(BinaryIpcChannelTest))]
    public class IsolatedProcessHostTest : RemoteHostFactoryTest
    {
        public override IHostFactory Factory
        {
            get { return new IsolatedProcessHostFactory(RuntimeAccessor.Instance); }
        }

        [Test]
        public void HostDoesNotTerminateAbruptlyIfUnhandledExceptionThrowsWithTheLegacyUnhandledExceptionPolicyEnabled()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.Configuration.LegacyUnhandledExceptionPolicyEnabled = true;

            using (IHost host = Factory.CreateHost(hostSetup, new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                host.GetHostService().Do<object, object>(ThrowUnhandledExceptionCallback, null);

                // Ping the host a few times to ensure that the process does not terminate abruptly.
                // In practice it may continue to service requests for a little while after the
                // unhandled exception occurs.
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    host.GetHostService().Ping();
                }
            }
        }
        private static object ThrowUnhandledExceptionCallback(object dummy)
        {
            Thread t = new Thread((ThreadStart)delegate
            {
                throw new Exception("Unhandled!");
            });

            t.Start();
            return null;
        }

        [Test]
        public void LaunchesX86HostWhenDemandedByProcessorArchitecture()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ProcessorArchitecture = ProcessorArchitecture.X86;

            using (IHost host = Factory.CreateHost(hostSetup, new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                string processName = host.GetHostService().Do<object, string>(GetHostProcessName, null);

                Assert.Contains(processName, "Gallio.Host.x86.exe");
            }
        }

        [Test]
        public void LaunchesMSILHostByDefault()
        {
            HostSetup hostSetup = new HostSetup();

            using (IHost host = Factory.CreateHost(hostSetup, new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                string processName = host.GetHostService().Do<object, string>(GetHostProcessName, null);

                Assert.Contains(processName, "Gallio.Host.exe");
            }
        }

        [Test]
        public void WhenRuntimeVersionIsDotNet20_RunsInDotNet20()
        {
            if (DotNetRuntimeSupport.IsUsingMono)
                Assert.Inconclusive("This test makes no sense on Mono.");

            HostSetup hostSetup = new HostSetup();
            hostSetup.RuntimeVersion = DotNetRuntimeSupport.InstalledDotNet20RuntimeVersion;

            StringWriter writer = new StringWriter();
            TextLogger logger = new TextLogger(writer);
            using (IHost host = Factory.CreateHost(hostSetup, logger))
            {
            }

            Assert.Contains(writer.ToString(), "CLR " + DotNetRuntimeSupport.InstalledDotNet20RuntimeVersion);
        }

        [Test]
        public void WhenRuntimeVersionIsDotNet40_RunsInDotNet40()
        {
            if (DotNetRuntimeSupport.IsUsingMono)
                Assert.Inconclusive("This test makes no sense on Mono.");
            if (DotNetRuntimeSupport.InstalledDotNet40RuntimeVersion == null)
                Assert.Inconclusive("This test requires .Net 4.0 to be installed.");

            HostSetup hostSetup = new HostSetup();
            hostSetup.RuntimeVersion = DotNetRuntimeSupport.InstalledDotNet40RuntimeVersion;

            StringWriter writer = new StringWriter();
            TextLogger logger = new TextLogger(writer);
            using (IHost host = Factory.CreateHost(hostSetup, logger))
            {
            }

            Assert.Contains(writer.ToString(), "CLR " + DotNetRuntimeSupport.InstalledDotNet40RuntimeVersion);
        }

        private static string GetHostProcessName(object dummy)
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }
    }
}
