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

using System.Diagnostics;
using Gallio.Model.Logging;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;
using TypeMock.Integration;

namespace Gallio.TypeMockIntegration.Tests
{
    [TestFixture]
    [TestsOn(typeof(TypeMockHost))]
    [TestsOn(typeof(TypeMockHostFactory))]
    [TestsOn(typeof(TypeMockProcessTask))]
    public class TypeMockHostIntegrationTest
    {
        [FixtureSetUp]
        public void TestFixtureSetUp()
        {
            if (!Service.IsInstalled)
                InterimAssert.Inconclusive("TypeMock does not appear to be installed so these tests will be skipped.");
        }

        [Test]
        public void TypeMockHostRunsWithTypeMockAttached()
        {
            TypeMockHostFactory factory = new TypeMockHostFactory(RuntimeAccessor.InstallationPath);

            using (IHost host = factory.CreateHost(new HostSetup(), new TestLogStreamLogger()))
            {
                HostAssemblyResolverHook.InstallCallback(host);

                bool isTypeMockRunning = host.GetHostService().Do<object, bool>(IsTypeMockRunning, null);
                Assert.IsTrue(isTypeMockRunning, "TypeMock should be attached to the host process.");
            }
        }

        private static bool IsTypeMockRunning(object dummy)
        {
            return TypeMockProcess.IsEnabled(Process.GetCurrentProcess());
        }
    }
}
