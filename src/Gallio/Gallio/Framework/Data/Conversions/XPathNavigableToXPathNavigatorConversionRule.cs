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
using System.Xml.XPath;

namespace Gallio.Framework.Data.Conversions
{
    /// <summary>
    /// Converts <see cref="IXPathNavigable" /> objects to <see cref="XPathNavigator" /> objects.
    /// </summary>
    public sealed class XPathNavigableToXPathNavigatorConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (typeof(IXPathNavigable).IsAssignableFrom(sourceType)
                && ! typeof(XPathNavigator).IsAssignableFrom(sourceType))
                return elementConverter.GetConversionCost(typeof(XPathNavigator), targetType).Add(ConversionCost.Typical);

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter)
        {
            IXPathNavigable node = (IXPathNavigable)sourceValue;
            return elementConverter.Convert(node.CreateNavigator(), targetType);
        }
    }
}