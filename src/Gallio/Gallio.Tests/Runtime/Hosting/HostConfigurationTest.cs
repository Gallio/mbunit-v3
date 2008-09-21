using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestsOn(typeof(HostConfiguration))]
    public class HostConfigurationTest
    {
        [Test]
        public void WriteToReencodesAccordingToTextWriter()
        {
            StringWriter writer = new StringWriter();
            Assert.AreEqual(Encoding.Unicode.EncodingName, writer.Encoding.EncodingName);

            HostConfiguration config = new HostConfiguration();
            config.WriteTo(writer);

            Assert.Contains(writer.ToString(), "encoding=\"utf-16\"");
        }

        [Test]
        public void WriteToReencodesAccordingToTextWriter_WhenConfigurationXmlContainsDifferentEncoding()
        {
            StringWriter writer = new StringWriter();
            Assert.AreEqual(Encoding.Unicode.EncodingName, writer.Encoding.EncodingName);

            HostConfiguration config = new HostConfiguration();
            config.ConfigurationXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration />";
            config.WriteTo(writer);

            Assert.Contains(writer.ToString(), "encoding=\"utf-16\"");
        }
    }
}
