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
using System.Linq;
using System.Text.RegularExpressions;
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
            Assert.Xml.Exists((TextReader)null, XmlPath.Element("Root"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_xml_should_throw_exception()
        {
            Assert.Xml.Exists((string)null, XmlPath.Element("Root"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_Exists_with_null_path_should_throw_exception()
        {
            Assert.Xml.Exists("<xml/>", null);
        }

        [Test]
        [Factory("GetTestCases")]
        public void Assert_Exists(string description, string xml, IXmlPath path, bool shouldPass)
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.Exists(xml, path, description));
            Assert.AreEqual(shouldPass ? 0 : 1, failures.Length);
        }

        private IEnumerable<object[]> GetTestCases()
        {
            yield return new object[]
            { 
                "Should find root element.",
                "<Root/>",
                XmlPath.Element("Root"),
                true
            };

            yield return new object[]
            { 
                "Should find child element at depth 1.",
                "<Root><Child/></Root>",
                XmlPath.Element("Root").Element("Child"),
                true
            };

            yield return new object[]
            { 
                "Should not find root element.",
                "<Root/>",
                XmlPath.Element("DoesNotExist"),
                false
            };

            yield return new object[]
            { 
                "Should find attribute in root element.",
                "<Root value='123' />",
                XmlPath.Element("Root").Attribute("value"),
                true
            };

            yield return new object[]
            { 
                "Should not find attribute in root element.",
                "<Root value='123' />",
                XmlPath.Element("Root").Attribute("doesNotExist"),
                false
            };

            yield return new object[]
            { 
                "Should find attribute in child element at depth 1.",
                "<Root><Child value='123'/></Root>",
                XmlPath.Element("Root").Element("Child").Attribute("value"),
                true
            };

            yield return new object[]
            { 
                "Should not find attribute in child element at depth 1.",
                "<Root><Child value='123'/></Root>",
                XmlPath.Element("Root").Element("Child").Attribute("doesNotExist"),
                false
            };

            yield return new object[]
            { 
                "Should find child element at depth 1 in a group.",
                "<Root><Child1/><Child2/><Child3/></Root>",
                XmlPath.Element("Root").Element("Child2"),
                true
            };

            yield return new object[]
            { 
                "Should not find child element at depth 1 in a group.",
                "<Root><Child1/><Child2/><Child3/></Root>",
                XmlPath.Element("Root").Element("DoesNotExist"),
                false
            };

            yield return new object[]
            { 
                "Should find attribute in a child element at depth 1.",
                "<Root><Child/><Child/><Child value='123'/></Root>",
                XmlPath.Element("Root").Element("Child").Attribute("value"),
                true
            };

            yield return new object[]
            { 
                "Should not find attribute in a child element at depth 1.",
                "<Root><Child/><Child/><Child value='123'/></Root>",
                XmlPath.Element("Root").Element("Child").Attribute("doesNotExist"),
                false
            };

            yield return new object[]
            { 
                "Should find child element at depth 2 in a group.",
                "<Root><Child/><Child><Node/></Child><Child/></Root>",
                XmlPath.Element("Root").Element("Child").Element("Node"),
                true
            };

            yield return new object[]
            { 
                "Should find deep attribute in a complex tree.",
                GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml"),
                XmlPath.Element("SolarSystem").Element("Planets").Element("Planet").Element("Satellites").Element("Satellite").Attribute("name"),
                true
            };


        }
    }
}