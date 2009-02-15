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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Ambience.Impl;
using MbUnit.Framework;
using System.Net;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(AmbienceServerConfiguration))]
    public class AmbienceServerConfigurationTest
    {
        [Test]
        public void Port_GetSet()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.AreEqual(Constants.DefaultPortNumber, config.Port);

            config.Port = 1111;
            Assert.AreEqual(1111, config.Port);
        }

        [Test]
        [Row(0), Row(65536)]
        public void Port_ThrowsIfOutOfRange(int value)
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(() => config.Port = value);
        }

        [Test]
        public void Credential_GetSet()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.AreEqual(Constants.AnonymousUserName, config.Credential.UserName);
            Assert.AreEqual(Constants.AnonymousPassword, config.Credential.Password);

            config.Credential = new NetworkCredential("abc", "def");
            Assert.AreEqual("abc", config.Credential.UserName);
            Assert.AreEqual("def", config.Credential.Password);
        }

        [Test]
        public void Credential_ThrowsIfValueIsNull()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.Credential = null);
        }

        [Test]
        public void DatabasePath_GetSet()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.AreEqual(Path.Combine(Constants.CommonAppDataFolderPath, Constants.DefaultDatabaseFileName), config.DatabasePath);

            config.DatabasePath = "somefile.db";
            Assert.AreEqual("somefile.db", config.DatabasePath);
        }

        [Test]
        public void DatabasePath_ThrowsIfValueIsNull()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.DatabasePath = null);
        }

        [Test]
        public void DatabasePath_ThrowsIfValueIsEmpty()
        {
            AmbienceServerConfiguration config = new AmbienceServerConfiguration();
            Assert.Throws<ArgumentException>(() => config.DatabasePath = "");
        }
    }
}
