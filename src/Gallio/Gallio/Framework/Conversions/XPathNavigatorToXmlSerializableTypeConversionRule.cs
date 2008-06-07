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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Gallio.Framework.Conversions
{
    /// <summary>
    /// Converts <see cref="XPathNavigator" /> objects to XML serializable
    /// objects that have the <see cref="XmlTypeAttribute" />.
    /// </summary>
    public sealed class XPathNavigatorToXmlSerializableTypeConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (typeof(XPathNavigator).IsAssignableFrom(sourceType)
                && targetType.IsDefined(typeof(XmlTypeAttribute), true)
                && targetType.IsDefined(typeof(XmlRootAttribute), true))
                return ConversionCost.Typical;

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter)
        {
            XPathNavigator navigator = (XPathNavigator)sourceValue;
            XmlSerializer serializer = new XmlSerializer(targetType);

            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream))
                    navigator.WriteSubtree(writer);

                stream.Position = 0;

                using (XmlReader reader = XmlReader.Create(stream))
                    return serializer.Deserialize(reader);
            }
        }
    }
}
