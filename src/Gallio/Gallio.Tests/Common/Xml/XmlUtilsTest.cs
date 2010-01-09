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
using System.Xml.XPath;
using Gallio.Common.Xml;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Xml
{
    [TestsOn(typeof(XmlUtils))]
    public class XmlUtilsTest
    {
        [Test]
        public void WriteToXPathDocument_WhenActionIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => XmlUtils.WriteToXPathDocument(null));
        }

        [Test]
        [Row("\U00010000", Description = "High unicode code point represented as surrogate pairs in UTF-16")]
        [Row("\0", Description = "Null character")]
        [Row("\ufffe", Description = "Byte Order Mark")]
        [Row("\u1100\u1102\u1103", Description = "16-bit Unicode")]
        public void WriteToXPathDocument_WhenWritingNonASCIICharacters_ShouldPreserveContentAndEncoding(string content)
        {
            XPathDocument document = XmlUtils.WriteToXPathDocument(xmlWriter =>
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("root");
                xmlWriter.WriteString(content);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
            });

            var rootNavigator = document.CreateNavigator().SelectSingleNode("/root");
            Assert.AreEqual(content, rootNavigator.Value);
        }
    }
}