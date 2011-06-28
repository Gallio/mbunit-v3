// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Container class for factory method which is consumed by Gallio test framework.
    /// </summary>
    public class XmlDataObjectFactory
    {
        /// <summary>
        /// Returns an IEnumerable which exposes an Enumerator for XmlDataObjects 
        /// for each specified Element in its XPath
        /// </summary>
        /// <param name="ElementList">Location of the Xml file</param>
        public static IEnumerable<dynamic> SingleDocument(List<XElement> ElementList)
        {
            // Cycle thru child elements and return a populated Data Object
            foreach (XElement xelement in ElementList)
            {
                yield return XmlDataObjectBuilder.EmitDataObject(xelement);
            }
        }

        /// <summary>
        /// Returns interface to an Enumerator which traverses the Cartesian Product of one-to-many lists of XElements.
        /// Each array of data contains a row of dynamic objects with one XElement from each of the lists of XElements.
        /// </summary>
        /// <param name="ListOfListsOfNodes">The List of Lists of Elements</param>
        public static IEnumerable<dynamic[]> MultipleDocuments(List<List<XElement>> ListOfListsOfNodes)
        {
            IEnumerable<XElement[]> cartesianProduct = MultipleListEnumerable.CartesianYield<XElement>(ListOfListsOfNodes);

            foreach (XElement[] output in cartesianProduct)
            {
                yield return output.Select<XElement, dynamic>(x => XmlDataObjectBuilder.EmitDataObject(x)).ToArray();
            }
        }
    }
}
