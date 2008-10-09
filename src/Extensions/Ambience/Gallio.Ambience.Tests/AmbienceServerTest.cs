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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(AmbienceServer))]
    public class AmbienceServerTest
    {
        private const int PortNumber = 65435;

        [Test]
        public void ConstructorThrowsIfConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AmbienceServer(null));
        }

        [Test]
        public void StartThrowsIfDisposed()
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()))
            {
                server.Dispose();

                Assert.Throws<ObjectDisposedException>(server.Start);
            }
        }

        [Test]
        public void StopThrowsIfDisposed()
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()))
            {
                server.Dispose();

                Assert.Throws<ObjectDisposedException>(server.Stop);
            }
        }

        [Test]
        public void ServerStoresDbInSpecifiedLocation()
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = "AmbienceClientTest.db"
            }))
            {
                File.Delete("AmbienceClientTest.db");

                server.Start();

                Assert.IsTrue(File.Exists("AmbienceClientTest.db"));
            }
        }

        [Test]
        public void ServerCreatesDbFolderWhenNeeded()
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = @"DbFolder\AmbienceClientTest.db"
            }))
            {
                if (Directory.Exists("DbFolder"))
                    Directory.Delete("DbFolder", true);

                server.Start();

                Assert.IsTrue(Directory.Exists("DbFolder"));
                Assert.IsTrue(File.Exists(@"DbFolder\AmbienceClientTest.db"));
            }
        }

        [Test]
        public void ServerGrantsAccessToAuthorizedUser()
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = @"AmbienceClientTest.db",
                Credential = { UserName = "Auth", Password = "Password" }
            }))
            {
                server.Start();

                Assert.DoesNotThrow(() => AmbienceClient.Connect(new AmbienceClientConfiguration() { Port = PortNumber, Credential = { UserName = "Auth", Password = "Password" } }));
            }
        }

        [Test]
        [Row("", "")]
        [Row("Auth", "BadPassword")]
        [Row("Unauth", "Password")]
        public void ServerDeniesAccessToUnauthorizedUser(string userName, string password)
        {
            using (AmbienceServer server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = @"AmbienceClientTest.db",
                Credential = { UserName = "Auth", Password = "Password" }
            }))
            {
                server.Start();

                Assert.Throws<AmbienceException>(() => AmbienceClient.Connect(new AmbienceClientConfiguration()
                    { Port = PortNumber, Credential = { UserName = userName, Password = password } }));
            }
        }
    }
}
