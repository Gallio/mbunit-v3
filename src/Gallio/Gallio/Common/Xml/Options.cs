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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Options for the XML-related equality assertions.
    /// </summary>
    [Flags]
    public enum Options
    {
        /// <summary>
        /// No equality options (the strictest).
        /// </summary>
        None = 0,

        /// <summary>
        /// Ignores the case of the name of the XML elements.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<ELEMENT/>" is equal to "<element/>".
        /// ]]></example>
        IgnoreElementsNameCase = 1,

        /// <summary>
        /// Ignores the case of the value of the XML elements.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<element>VALUE</element>" is equal to "<element>value</element>".
        /// ]]></example>
        IgnoreElementsValueCase = 2,

        /// <summary>
        /// Ignores the order of XML elements with the same parent.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<parent><child1/><child2/></parent>" is equal to "<parent><child2/><child1/></parent>".
        /// ]]></example>
        IgnoreElementsOrder = 4,

        /// <summary>
        /// Ignores the case of the name of the XML attributes.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<element ATTRIBUTE='value'/>" is equal to "<element attribute='value'/>".
        /// ]]></example>
        IgnoreAttributesNameCase = 8,

        /// <summary>
        /// Ignores the case of the value of the XML attributes.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<element attribute='VALUE'/>" is equal to "<element attribute='value'/>".
        /// ]]></example>
        IgnoreAttributesValueCase = 16,

        /// <summary>
        /// Ignores the order of the attributes defined in the same element.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<element attr1='x' attr2='y'/>" is equal to "<element attr2='y' attr1='x'/>".
        /// ]]></example>
        IgnoreAttributesOrder = 32,

        /// <summary>
        /// Ignores the comment tags.
        /// </summary>
        /// <example><![CDATA[ 
        /// "<!-- This is a comment... -->".
        /// ]]></example>
        IgnoreComments = 64,
    }
}
