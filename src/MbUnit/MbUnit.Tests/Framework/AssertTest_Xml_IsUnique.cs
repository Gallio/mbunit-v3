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
    public class AssertTest_Xml_IsUnique : BaseAssertTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Assert_IsUnique_with_null_reader_should_throw_exception()
        {
            Assert.Xml.IsUnique((TextReader)null, XmlPath.Element("Root"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_IsUnique_with_null_xml_should_throw_exception()
        {
            Assert.Xml.IsUnique((string)null, XmlPath.Element("Root"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Assert_IsUnique_with_null_path_should_throw_exception()
        {
            Assert.Xml.IsUnique("<xml/>", null);
        }

        [Test]
        public void Assert_IsUnique_passes()
        {
            var xml = GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml");
            Assert.Xml.IsUnique(xml, XmlPath.Element("SolarSystem").Element("Planets"));
        }

        [Test]
        public void Assert_IsUnique_fails_because_the_searched_item_does_not_exist()
        {
            var xml = GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml");
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.IsUnique(xml, XmlPath.Element("SolarSystem").Element("Moons")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the XML fragment to contain only once the searched XML element or attribute, but none was found.", failures[0].Description);
            Assert.AreEqual("<SolarSystem><Moons>", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("0", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Assert_IsUnique_fails_because_the_searched_item_exists_several_times()
        {
            var xml = GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml");
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.IsUnique(xml, XmlPath.Element("SolarSystem").Element("Planets").Element("Planet")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the XML fragment to contain only once the searched XML element or attribute, But several were found.", failures[0].Description);
            Assert.AreEqual("<SolarSystem><Planets><Planet>", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("8", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
    }
}