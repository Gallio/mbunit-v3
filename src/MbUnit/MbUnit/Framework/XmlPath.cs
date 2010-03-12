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
using System.Text;
using System.Collections.ObjectModel;
using Gallio.Common.Xml;

namespace MbUnit.Framework
{
    /// <summary>
    /// Represents the path to an element or an attribute in a XML fragment.
    /// </summary>
    /// <example>
    /// Consider the following XML fragment:
    /// <![CDATA[
    /// <Root>
    ///   <Parent>
    ///     <Child value="123"/>
    ///   </Parent>
    /// <Root>
    /// ]]>
    /// The path to the attribute <c>value</c> of the element <c>Child</c> is specified as follows:
    /// <![CDATA[
    /// var path = XmlPath.Element("Root").Element("Parent").Element("Child").Attribute("value");
    /// ]]></example>
    public class XmlPath
    {
        /// <summary>
        /// Starts the construction of a path.
        /// </summary>
        /// <param name="elementName">The name of the first element.</param>
        /// <returns>A path that can be further extended.</returns>
        public static IXmlPathLooseOpen Element(string elementName)
        {
            return XmlPathRoot.Element(elementName);
        }
    }
}
