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
