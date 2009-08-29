// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Xml;

namespace MbUnit.Framework
{
    /// <summary>
    /// Optional settings for the XML equality assertions.
    /// </summary>
    /// <seealso cref="Assert.Xml.AreEqual(string,string)"/>
    public abstract class XmlOptions
    {
        private readonly Options value;

        /// <summary>
        /// Gets the inner value.
        /// </summary>
        internal Options Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preset"></param>
        protected XmlOptions(Options preset)
        {
            value = preset;
        }

        /// <summary>
        /// The default settings for the XML equality assertions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Those settings specify to ignore the order of the elements and the attributes located
        /// in the same parent element, and to ignore the comment tags as well.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal genus='Rangifer' species='tarandus'>Peary Caribou</Animal>"
        ///     string actual = "<Animal species='tarandus' genus='Rangifer'>Peary Caribou</Animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Default); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public readonly static XmlOptions Default = CustomXmlOptions.Instance.IgnoreAllOrder.IgnoreComments;

        /// <summary>
        /// The strictest settings for the XML equality assertions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The strictest settings expect the elements and the attributes to be in the same order as
        /// in the expected XML data, and makes case sensitive string comparisons for the name and the
        /// value of the elements and the attributes. Comment tags are included in the comparison 
        /// process as well.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal>" +
        ///                       "  <!-- Some details... -->
        ///                       "  <Name value='Peary Caribou'/>" +
        ///                       "  <Genus/>" +
        ///                       "  <Species value='tarandus'/>" +
        ///                       "</Animal>"
        ///     string actual =   "<Animal>" +
        ///                       "  <!-- Some details... -->
        ///                       "  <Name value='Peary Caribou'/>" 
        ///                       "  <Genus></Genus>" +
        ///                       "  <Species value='tarandus'/>" +
        ///                       "</Animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Strict); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public readonly static XmlOptions Strict = CustomXmlOptions.Instance;

        /// <summary>
        /// The loosest settings for the XML equality assertions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The settings ignored the order of the elements and the attributes located in the
        /// same parent element. It makes case insensitive string comparisons for the name and 
        /// the value of the attributes and the elements. It ignored the comment tags as well.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal>" +
        ///                       "  <!-- Some details... -->
        ///                       "  <Name value='Peary Caribou'/>" +
        ///                       "  <Genus value='Rangifer'/>" +
        ///                       "  <Species value='tarandus'/>" +
        ///                       "</Animal>"
        ///     string actual =   "<animal>" +
        ///                       "  <genus VALue='rangifer'/>" +
        ///                       "  <NAme value='Peary CaRibou'/>" 
        ///                       "  <Species VALUE='TARANDUS'/>" +
        ///                       "</animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Loose); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public readonly static XmlOptions Loose = CustomXmlOptions.Instance.IgnoreAllOrder.IgnoreAllCase.IgnoreComments;

        /// <summary>
        /// Custom settings for the XML equality assertions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Lets you define your own equality settings. Combine the different available options together
        /// to specify how the equality between 2 provided XML fragments must be done.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal>Peary Caribou</Animal>"
        ///     string actual = "<Animal>Peary Caribou</Animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreComments.IgnoreElementsCaseName.IgnoreAttributesOrder); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public readonly static CustomXmlOptions Custom = CustomXmlOptions.Instance;
    }

    /// <summary>
    /// Customizable optional settings for the XML equality assertions.
    /// </summary>
    /// <seealso cref="Assert.Xml.AreEqual(string,string)"/>
    public sealed class CustomXmlOptions : XmlOptions
    {
        internal readonly static CustomXmlOptions Instance = new CustomXmlOptions(Options.None);

        internal CustomXmlOptions(Options preset)
            : base(preset)
        {
        }

        /// <summary>
        /// Indicates that the <b>name</b> of the XML elements must be compared with case insensitivity.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<ANIMAL>Peary Caribou</ANIMAL>"
        ///     string actual = "<animal>Peary Caribou</animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreElementsNameCase); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreElementsNameCase
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreElementsNameCase);
            }
        }

        /// <summary>
        /// Indicates that the <b>value</b> of the XML elements must be compared with case insensitivity.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal>Peary Caribou</Animal>"
        ///     string actual = "<Animal>PEaRY CaRiboU</Animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreElementsValueCase); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreElementsValueCase
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreElementsValueCase);
            }
        }

        /// <summary>
        /// Indicates that the <b>name</b> of the XML attributes must be compared with case insensitivity.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal name='Peary Caribou'/>"
        ///     string actual = "<Animal NAME='Peary Caribou'/>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreAttributesNameCase); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreAttributesNameCase
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreAttributesNameCase);
            }
        }

        /// <summary>
        /// Indicates that the <b>value</b> of the XML attributes must be compared with case insensitivity.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal name='Peary Caribou'/>"
        ///     string actual = "<Animal name='PEaRY CaRiboU'/>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreAttributesValueCase); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreAttributesValueCase
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreAttributesValueCase);
            }
        }

        /// <summary>
        /// Indicates that the order of the child elements within a given parent element must be ignored.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal>" +
        ///                       "  <Name value='Peary Caribou'/>" +   // 1st
        ///                       "  <Genus value='Rangifer'/>" +       // 2nd
        ///                       "  <Species value='tarandus'/>" +     // 3rd
        ///                       "</Animal>"
        ///     string actual =   "<Animal>" +
        ///                       "  <Genus value='Rangifer'/>" +       // 2nd -> 1st!
        ///                       "  <Species value='tarandus'/>" +     // 3rd -> 2nd!
        ///                       "  <Name value='Peary Caribou'/>" +   // 1st -> 3rd!
        ///                       "</Animal>"
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreElementsOrder); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreElementsOrder
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreElementsOrder);
            }
        }

        /// <summary>
        /// Indicates that the order of the attributes within the same element must be ignored.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal" +
        ///                       "  name='Peary Caribou'" +    // 1st
        ///                       "  genus='Rangifer'" +        // 2nd
        ///                       "  species='tarandus' />" +   // 3rd
        ///     string actual =   "<Animal" +
        ///                       "  genus='Rangifer'" +        // 2nd -> 1st!
        ///                       "  species='tarandus'" +      // 3rd -> 2nd!
        ///                       "  name='Peary Caribou' />" + // 1st -> 3rd!
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreAttributesOrder); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreAttributesOrder
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreAttributesOrder);
            }
        }

        /// <summary>
        /// Indicates that the comment tags should be ignored during the comparison.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// public void MyXmlTest()
        /// {
        ///     string expected = "<Animal name='Peary Caribou'><!-- Living in herds numbering less than 20. --></Animal>" +;
        ///     string actual = "<Animal name='Peary Caribou'><!-- Unknown surviving number. --></Animal>" +;
        ///     Assert.Xml.AreEqual(expected, actual, XmlOptions.Custom.IgnoreComments); // Pass!
        /// }
        /// ]]></code>
        /// </example>
        public CustomXmlOptions IgnoreComments
        {
            get
            {
                return new CustomXmlOptions(Value | Options.IgnoreComments);
            }
        }

        /// <summary>
        /// Combines the options <see cref="IgnoreElementsNameCase"/> and <see cref="IgnoreElementsValueCase"/>
        /// </summary>
        public CustomXmlOptions IgnoreElementsCase
        {
            get
            {
                return IgnoreElementsNameCase.IgnoreElementsValueCase;
            }
        }

        /// <summary>
        /// Combines the options <see cref="IgnoreAttributesNameCase"/> and <see cref="IgnoreAttributesValueCase"/>
        /// </summary>
        public CustomXmlOptions IgnoreAttributesCase
        {
            get
            {
                return IgnoreAttributesNameCase.IgnoreAttributesValueCase;
            }
        }

        /// <summary>
        /// Combines the options <see cref="IgnoreElementsCase"/> and <see cref="IgnoreAttributesCase"/>
        /// </summary>
        public CustomXmlOptions IgnoreAllCase
        {
            get
            {
                return IgnoreElementsCase.IgnoreAttributesCase;
            }
        }

        /// <summary>
        /// Combines the options <see cref="IgnoreElementsOrder"/> and <see cref="IgnoreAttributesOrder"/>
        /// </summary>
        public CustomXmlOptions IgnoreAllOrder
        {
            get
            {
                return IgnoreElementsOrder.IgnoreAttributesOrder;
            }
        }
    }
}
