using System.Xml;
using System.Xml.XPath;
using Gallio.Framework.Data.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Conversions
{
    [TestFixture]
    [TestsOn(typeof(StringToXmlDocumentConversionRule))]
    public class StringToXmlDocumentConversionRuleTest : BaseConversionRuleTest<StringToXmlDocumentConversionRule>
    {
        [Test]
        public void DirectConversion()
        {
            string sourceValue = "<root />";

            XmlDocument targetValue = (XmlDocument)Converter.Convert(sourceValue, typeof(XmlDocument));
            Assert.AreEqual("<root />", targetValue.OuterXml);
        }

        [Test]
        public void UnsupportedConversion()
        {
            Assert.IsFalse(Converter.CanConvert(typeof(XmlDocument), typeof(string)));
        }
    }
}