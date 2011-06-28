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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Helpful utility methods to aid in working around the XmlDataObject's idiosynracies.
    /// </summary>
    public class XmlDataObjectUtility
    {
        /// <summary>
        /// Method for finding an immediate child element that has an attribute with a certain value.
        /// </summary>
        /// <param name="ChildElements">Either an XmlDataObject or a List of XmlDataObjects</param>
        /// <param name="AttributeName">Name of Xml attribute we're querying</param>
        /// <param name="AttributeValue">Expect value of the Xml attribute</param>
        /// <returns>XmlDataObject Element</returns>
        public static dynamic FindFirstElementByAttribute(dynamic ChildElements, string AttributeName, string AttributeValue)
        {
            List<XmlDataObject> ListOfElements = XmlDataObject.AsList(ChildElements);

            foreach (XmlDataObject Element in ListOfElements)
            {
                if (Element.GetMember(AttributeName) as string == AttributeValue)
                    return Element;
            }

            // If we have a value type i.e. a leaf, then it obviously won't have attributes
            return null;
        }

        /// <summary>
        /// Takes sequence (or solitary member) of Elements and dumps the values into string List
        /// </summary>
        public static List<string> ElementValuesToStringList(dynamic ChildElements)
        {
            List<XmlDataObject> ListOfElements = XmlDataObject.AsList(ChildElements);
            List<string> retval = new List<string>();

            foreach (dynamic Element in XmlDataObject.AsList(ChildElements))
                retval.Add(Element.Value);

            return retval.Cast<string>().ToList<string>();
        }
    }
}
