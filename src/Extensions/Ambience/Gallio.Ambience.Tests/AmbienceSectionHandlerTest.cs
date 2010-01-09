// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Text;
using System.Xml;
using Gallio.Ambience.Impl;
using MbUnit.Framework;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(AmbienceSectionHandler))]
    public class AmbienceSectionHandlerTest
    {
        [Test]
        [Row("<ambience></ambience>",
            "localhost", Constants.DefaultPortNumber, Constants.AnonymousUserName, Constants.AnonymousPassword)]
        [Row(@"<ambience><defaultClient hostName=""SomeHost"" /></ambience>",
            "SomeHost", Constants.DefaultPortNumber, Constants.AnonymousUserName, Constants.AnonymousPassword)]
        [Row(@"<ambience><defaultClient hostName=""SomeHost"" port=""50"" /></ambience>",
            "SomeHost", 50, Constants.AnonymousUserName, Constants.AnonymousPassword)]
        [Row(@"<ambience><defaultClient hostName=""SomeHost"" port=""50"" userName=""Test"" /></ambience>",
            "SomeHost", 50, "Test", Constants.AnonymousPassword)]
        [Row(@"<ambience><defaultClient hostName=""SomeHost"" port=""50"" userName=""Test"" password=""Password"" /></ambience>",
            "SomeHost", 50, "Test", "Password")]
        public void ParsesFromXml(string xml, string defaultClientHostName, int defaultClientPort,
            string defaultClientUserName, string defaultClientPassword)
        {
            AmbienceSectionHandler handler = new AmbienceSectionHandler();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var section = (AmbienceConfigurationSection) handler.Create(null, null, doc.FirstChild);

            Assert.AreEqual(defaultClientHostName, section.DefaultClientConfiguration.HostName);
            Assert.AreEqual(defaultClientPort, section.DefaultClientConfiguration.Port);
            Assert.AreEqual(defaultClientUserName, section.DefaultClientConfiguration.Credential.UserName);
            Assert.AreEqual(defaultClientPassword, section.DefaultClientConfiguration.Credential.Password);
        }
    }
}
