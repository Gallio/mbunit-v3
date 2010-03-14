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
using System.Linq;
using System.Text.RegularExpressions;
using Gallio.Common.Xml.Paths;
using Gallio.Framework.Assertions;
using MbUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;
using System.Reflection;
using System.IO;
using Gallio.Common.Xml;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Xml_Exists : BaseAssertTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_reader_should_throw_exception()
        {
            Assert.Xml.Exists((TextReader)null, XmlPath.Element("Root"), XmlOptions.Default);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_xml_should_throw_exception()
        {
            Assert.Xml.Exists((string)null, XmlPath.Element("Root"), XmlOptions.Default);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_path_should_throw_exception()
        {
            Assert.Xml.Exists("<xml/>", (IXmlPathLoose)null, XmlOptions.Default);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_text_path_should_throw_exception()
        {
            Assert.Xml.Exists("<xml/>", (string)null, XmlOptions.Default);
        }

        [Test]
        [Factory("GetTestCases")]
        public void Assert_Exists(string description, string xml, string path, string expectedValue, XmlOptions options, bool shouldPass)
        {
            AssertionFailure[] failures = Capture(() => Assert.Xml.Exists(xml, path, options, expectedValue, description));
            Assert.AreEqual(shouldPass ? 0 : 1, failures.Length);
        }

        private IEnumerable<object[]> GetTestCases()
        {
            yield return new object[]
            { 
                "Should find root element.",
                "<Root/>",
                "/Root", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should find root element (case insensitive).",
                "<Root/>",
                "/ROOT", null,
                XmlOptions.Custom.IgnoreElementsNameCase, true
            };

            yield return new object[]
            { 
                "Should not find root element.",
                "<Root/>",
                "/DoesNotExist", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should not find root element (case sensitive).",
                "<Root/>",
                "/ROOT", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find child element at depth 1.",
                "<Root><Child/></Root>",
                "/Root/Child", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should find attribute in root element.",
                "<Root value='123' />",
                "/Root:value", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should find attribute in root element (case insensitive).",
                "<Root value='123' />",
                "/Root:VALUE", null,
                XmlOptions.Custom.IgnoreAttributesNameCase, true
            };

            yield return new object[]
            { 
                "Should not find attribute in root element.",
                "<Root value='123' />",
                "/Root:doesNotExist", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should not find attribute in root element (case sensitive).",
                "<Root value='123' />",
                "/Root:VALUE", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find attribute in child element at depth 1.",
                "<Root><Child value='123'/></Root>",
                "/Root/Child:value", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find attribute in child element at depth 1.",
                "<Root><Child value='123'/></Root>",
                "/Root/Child:doesNotExist", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find child element at depth 1 in a group.",
                "<Root><Child1/><Child2/><Child3/></Root>",
                "/Root/Child2", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find child element at depth 1 in a group.",
                "<Root><Child1/><Child2/><Child3/></Root>",
                "/Root/DoesNotExist", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find attribute in a child element at depth 1.",
                "<Root><Child/><Child/><Child value='123'/></Root>",
                "/Root/Child:value", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find attribute in a child element at depth 1.",
                "<Root><Child/><Child/><Child value='123'/></Root>",
                "/Root/Child:doesNotExist", null,
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find child element at depth 2 in a group.",
                "<Root><Child/><Child><Node/></Child><Child/></Root>",
                "/Root/Child/Node", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should find deep attribute in a complex tree.",
                GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml"),
                "/SolarSystem/Planets/Planet/Satellites/Satellite:name", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should child at lower depth than maximum depth in the tree.",
                "<Root><Parent><Child/></Parent></Root>",
                "/Root/Parent", null,
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should find root element with value.",
                "<Root>Hello</Root>",
                "/Root", "Hello",
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find root element with wrong value.",
                "<Root>Hello</Root>",
                "/Root", "Good Morning",
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find root element with value (case insensitive).",
                "<Root>Hello</Root>",
                "/Root", "HELLO",
                XmlOptions.Custom.IgnoreAllCase, true
            };

            yield return new object[]
            { 
                "Should find attribute value in root element.",
                "<Root value='Hello' />",
                "/Root:value", "Hello",
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find attribute with wrong value in root element.",
                "<Root value='Hello' />",
                "/Root:value", "Goodbye",
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should find deep attribute name/value in a complex tree.",
                GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml"),
                "/SolarSystem/Planets/Planet/Satellites/Satellite:name", "Tethys",
                XmlOptions.Default, true
            };

            yield return new object[]
            { 
                "Should not find deep attribute name with wrong value in a complex tree.",
                GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml"),
                "/SolarSystem/Planets/Planet/Satellites/Satellite:name", "Vador's Dark Star",
                XmlOptions.Default, false
            };

            yield return new object[]
            { 
                "Should not find attribute value in a child element at depth 1.",
                "<Root><Child/></Root>",
                "/Root/Child:attribute", null,
                XmlOptions.Default, false
            };
        }
    }
}