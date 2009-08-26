using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// Options for the XML-related equality assertions.
    /// </summary>
    /// <seealso cref="Assert.Xml.AreEqual(string,string)"/>
    [Flags]
    public enum XmlEqualityOptions
    {
        /// <summary>
        /// Default equality options.
        /// </summary>
        /// <remarks>
        /// The default options specify to ignore the elements/attributes order, but not the case. It is
        /// equal to <see cref="IgnoreAllOrder"/>.
        /// </remarks>
        Default = IgnoreAllOrder,

        /// <summary>
        /// Strict equality.
        /// </summary>
        Strict = 0,

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
        /// Ignores the case of the name and the value of the XML elements.
        /// </summary>
        /// <remarks>
        /// Combines <see cref="IgnoreElementsNameCase"/> and <see cref="IgnoreElementsValueCase"/>.
        /// </remarks>
        IgnoreElementsCase = IgnoreElementsNameCase | IgnoreElementsValueCase,

        /// <summary>
        /// Ignores the case of the name and the value of the XML attributes.
        /// </summary>
        /// <remarks>
        /// Combines <see cref="IgnoreAttributesNameCase"/> and <see cref="IgnoreAttributesValueCase"/>.
        /// </remarks>
        IgnoreAttributesCase = IgnoreAttributesNameCase | IgnoreAttributesValueCase,

        /// <summary>
        /// Ignores the case of the name and the values of the XML elements and attributes.
        /// </summary>
        /// <remarks>
        /// Combines <see cref="IgnoreElementsCase"/> and <see cref="IgnoreAttributesCase"/>.
        /// </remarks>
        IgnoreAllCase = IgnoreElementsCase | IgnoreAttributesCase,

        /// <summary>
        /// Ignores the order of the elements with the same parents and the order of the
        /// attributes within the same elements.
        /// </summary>
        /// <remarks>
        /// Combines <see cref="IgnoreElementsOrder"/> and <see cref="IgnoreAttributesOrder"/>.
        /// </remarks>
        IgnoreAllOrder = IgnoreElementsOrder | IgnoreAttributesOrder,

        /// <summary>
        /// Ignores any order and any case.
        /// </summary>
        /// <remarks>
        /// Combines <see cref="IgnoreAllCase"/> and <see cref="IgnoreElementsOrder"/>.
        /// </remarks>
        Loose = IgnoreAllCase | IgnoreElementsOrder
    }
}
