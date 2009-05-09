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
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    public abstract class AbstractHostFactoryTest : BaseTestWithMocks
    {
        public abstract IHostFactory Factory { get; }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            Factory.CreateHost(null, new MarkupStreamLogger(TestLog.Default));
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
            IHostService hostService;
            using (host = Factory.CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                // Should work fine.
                hostService = host.GetHostService();
                hostService.Ping();
            }

            // Should fail.
            Assert.Throws<Exception>(delegate { hostService.Ping(); });
        }

        [Test]
        public void CreateInstanceCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                Type remoteType = typeof(ArrayList);
                ObjectHandle handle = host.GetHostService().CreateInstance(remoteType.Assembly.FullName, remoteType.FullName);

                Assert.IsInstanceOfType(remoteType, handle.Unwrap());
            }
        }

        [Test]
        public void CreateInstanceFromCreatesAValidObjectHandle()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                Type serviceType = typeof(RemoteHostFactoryTest.TestService);
                RemoteHostFactoryTest.TestService serviceProxy = (RemoteHostFactoryTest.TestService)host.GetHostService().CreateInstanceFrom(
                    AssemblyUtils.GetAssemblyLocalPath(serviceType.Assembly), serviceType.FullName).Unwrap();

                Assert.AreEqual(42, serviceProxy.Add(23, 19));
            }
        }

        [Test]
        public void HostRunsInRequestedWorkingDirectory()
        {
            string oldWorkingDirectory = Environment.CurrentDirectory;

            HostSetup hostSetup = new HostSetup();
            hostSetup.WorkingDirectory = Path.GetTempPath();

            using (IHost host = Factory.CreateHost(hostSetup, new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);
                string remoteWorkingDirectory = host.GetHostService().Do<object, string>(GetWorkingDirectory, null);
                AssertArePathsEqualIgnoringFinalBackslash(Path.GetTempPath(), remoteWorkingDirectory);
            }

            Assert.AreEqual(oldWorkingDirectory, Environment.CurrentDirectory,
                "Current working directory of the calling process should be unchanged or at least restored once the host is disposed.");
        }

        protected static string GetWorkingDirectory(object ignored)
        {
            return Environment.CurrentDirectory;
        }

        protected static void AssertArePathsEqualIgnoringFinalBackslash(string expected, string actual)
        {
            Assert.AreEqual(AddFinalBackslashIfAbsent(expected), AddFinalBackslashIfAbsent(actual));
        }

        private static string AddFinalBackslashIfAbsent(string value)
        {
            return value.EndsWith("\\") ? value : value + '\\';
        }
    }
}
