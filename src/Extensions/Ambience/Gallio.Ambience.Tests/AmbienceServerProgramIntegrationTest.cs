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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Gallio.Ambience.Server;
using Gallio.Concurrency;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Ambience.Tests
{
    [TestFixture]
    [TestsOn(typeof(AmbienceServerProgram))]
    public class AmbienceServerProgramIntegrationTest
    {
        private const int PortNumber = 65435;
        
        [Test]
        public void AmbienceServerRunsWithSpecifiedOptions()
        {
            ProcessTask task = StartAmbienceServer("/db:IntegrationTest.db /p:" + PortNumber + " /u:Test /pw:LetMeIn");
 
            var config = new AmbienceClientConfiguration()
            {
                Port = PortNumber,
                Credential = new NetworkCredential("Test", "LetMeIn")
            };

            AmbienceClient client = AmbienceClient.Connect(config);
            IAmbientDataContainer container = client.Container;

            container.DeleteAll();

            container.Store(new Item() { Name = "foo", Value = 42 });
            container.Store(new Item() { Name = "bar", Value = 40 });

            Assert.AreEqual("foo", (from Item x in container where x.Value == 42 select x.Name).Single());
            Assert.AreEqual(0, (from Item x in container where x.Value == 0 select x).Count());

            task.Abort();
        }

        private ProcessTask StartAmbienceServer(string arguments)
        {
            string workingDirectory = Path.GetDirectoryName((AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)));
            string executablePath = Path.Combine(workingDirectory, "Gallio.Ambience.Server.exe");

            ProcessTask task = Tasks.StartProcessTask(executablePath, arguments, workingDirectory);
            Thread.Sleep(10000);
            return task;
        }
    }
}
