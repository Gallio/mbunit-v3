using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
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
