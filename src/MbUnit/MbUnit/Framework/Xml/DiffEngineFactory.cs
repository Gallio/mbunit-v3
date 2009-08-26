using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Diffing engine abstract factory.
    /// </summary>
    public static class DiffEngineFactory
    {
        /// <summary>
        /// Makes a diffing engine for collections of attributes.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>The resulting diffing engine.</returns>
        public static IDiffEngine<AttributeCollection> ForAttributes(AttributeCollection expected, AttributeCollection actual, Path path, XmlEqualityOptions options)
        {
            if ((options & XmlEqualityOptions.IgnoreAttributesOrder) != 0)
            {
                return new DiffEngineForUnorderedAttributes(expected, actual, path, options);
            }
            else
            {
                return new DiffEngineForOrderedItems<AttributeCollection, Attribute>(expected, actual, path, options, "attribute");
            }
        }

        /// <summary>
        /// Makes a diffing engine for collections of elements.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>The resulting diffing engine.</returns>
        public static IDiffEngine<ElementCollection> ForElements(ElementCollection expected, ElementCollection actual, Path path, XmlEqualityOptions options)
        {
            if ((options & XmlEqualityOptions.IgnoreElementsOrder) != 0)
            {
                return new DiffEngineForUnorderedElements(expected, actual, path, options);
            }
            else
            {
                return new DiffEngineForOrderedItems<ElementCollection, Element>(expected, actual, path, options, "element");
            }
        }
    }
}
