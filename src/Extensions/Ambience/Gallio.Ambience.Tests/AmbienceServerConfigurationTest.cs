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
            Assert.AreEqual(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "Default.db"), config.DatabasePath);

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
