using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Ambience.Impl;
using MbUnit.Framework;
using System.Net;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(AmbienceClientConfiguration))]
    public class AmbienceClientConfigurationTest
    {
        [Test]
        public void HostName_GetSet()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.AreEqual("localhost", config.HostName);

            config.HostName = "somehost";
            Assert.AreEqual("somehost", config.HostName);
        }

        [Test]
        public void HostName_ThrowsIfValueIsNull()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.HostName = null);
        }

        [Test]
        public void HostName_ThrowsIfValueIsEmpty()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.Throws<ArgumentException>(() => config.HostName = "");
        }

        [Test]
        public void Port_GetSet()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.AreEqual(Constants.DefaultPortNumber, config.Port);

            config.Port = 1111;
            Assert.AreEqual(1111, config.Port);
        }

        [Test]
        [Row(0), Row(65536)]
        public void Port_ThrowsIfOutOfRange(int value)
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(() => config.Port = value);
        }

        [Test]
        public void Credential_GetSet()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.AreEqual(Constants.AnonymousUserName, config.Credential.UserName);
            Assert.AreEqual(Constants.AnonymousPassword, config.Credential.Password);

            config.Credential = new NetworkCredential("abc", "def");
            Assert.AreEqual("abc", config.Credential.UserName);
            Assert.AreEqual("def", config.Credential.Password);
        }

        [Test]
        public void Credential_ThrowsIfValueIsNull()
        {
            AmbienceClientConfiguration config = new AmbienceClientConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.Credential = null);
        }
    }
}
