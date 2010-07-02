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
using System.Xml.XPath;

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Converts an <see cref="XPathNavigator" /> into a <see cref="String" /> by value or outer xml.
    /// </summary>
    public sealed class XPathNavigatorToStringConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (typeof(XPathNavigator).IsAssignableFrom(sourceType))
                return elementConverter.GetConversionCost(typeof(string), targetType).Add(ConversionCost.Typical);

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter, bool nullable)
        {
            XPathNavigator node = (XPathNavigator)sourceValue;
            return elementConverter.Convert(node.Value, targetType);
        }
    }
}