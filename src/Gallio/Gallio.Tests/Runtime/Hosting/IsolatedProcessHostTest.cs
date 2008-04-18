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
using System.Threading;
using Gallio.Framework.Utilities;
using Gallio.Runtime.Hosting;
using Gallio.Tests.Runtime.Remoting;
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
            get { return new IsolatedProcessHostFactory(); }
        }

        [Test]
        public void HostDoesNotTerminateAbruptlyIfUnhandledExceptionThrowsWithTheLegacyUnhandledExceptionPolicyEnabled()
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.Configuration.LegacyUnhandledExceptionPolicyEnabled = true;

            using (IHost host = Factory.CreateHost(hostSetup, new LogStreamLogger()))
            {
                HostAssemblyResolverHook.Install(host);
                host.DoCallback(ThrowUnhandledExceptionCallback);

                // Ping the host a few times to ensure that the process does not terminate abruptly.
                // In practice it may continue to service requests for a little while after the
                // unhandled exception occurs.
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    host.Ping();
                }
            }
        }
        private static void ThrowUnhandledExceptionCallback()
        {
            Thread t = new Thread((ThreadStart)delegate
            {
                throw new Exception("Unhandled!");
            });

            t.Start();
        }
    }
}
