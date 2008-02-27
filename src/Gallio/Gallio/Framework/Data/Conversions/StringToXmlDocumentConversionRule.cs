using System;
using System.Xml;
using System.Xml.XPath;

namespace Gallio.Framework.Data.Conversions
{
    /// <summary>
    /// Converts a <see cref="string" /> into an <see cref="XmlDocument" /> assuming
    /// the string is valid xml.
    /// </summary>
    public sealed class StringToXmlDocumentConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (typeof(string).IsAssignableFrom(sourceType))
                return elementConverter.GetConversionCost(typeof(XmlDocument), targetType).Add(ConversionCost.Typical);

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml((string)sourceValue);
            return elementConverter.Convert(doc, targetType);
        }
    }
}