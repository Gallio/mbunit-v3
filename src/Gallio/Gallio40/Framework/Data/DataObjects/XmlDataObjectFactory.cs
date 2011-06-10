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
