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
using Gallio.Common.Xml;
using Gallio.Framework;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml.Paths;
using System.Collections.Generic;

namespace Gallio.Tests.Common.Xml.Paths
{
    [TestFixture]
    [TestsOn(typeof(IXmlPath))]
    public class IXmlPathTest
    {
        #region Loose paths building

        [Test]
        [ExpectedArgumentNullException]
        public void Element_with_null_name_should_throw_exception()
        {
            XmlPathRoot.Element(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void Element_with_empty_name_should_throw_exception()
        {
            XmlPathRoot.Element(String.Empty);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Attribute_with_null_name_should_throw_exception()
        {
            XmlPathRoot.Element("Element").Attribute(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void Attribute_with_empty_name_should_throw_exception()
        {
            XmlPathRoot.Element("Element").Attribute(String.Empty);
        }

        [Test]
        public void Formats_empty_loose_path()
        {
            var actual = XmlPathRoot.Empty.ToString();
            Assert.AreEqual("/", actual);
        }

        [Test]
        public void Formats_loose_path_with_one_element()
        {
            var actual = XmlPathRoot.Element("Root").ToString();
            Assert.AreEqual("/Root", actual);
        }

        [Test]
        public void Formats_complex_loose_path()
        {
            var actual = XmlPathRoot.Element("Root").Element("Parent").Element("Child").Attribute("value").ToString();
            Assert.AreEqual("/Root/Parent/Child:value", actual);
        }

        #endregion

        #region Loose paths parsing

        [Test]
        [ExpectedArgumentNullException]
        public void Parse_null_should_throw_exception()
        {
            XmlPathRoot.Parse(null);
        }

        [Test]
        [Row("")]
        [Row("blabla")]
        [Row("/x/y:a/z")]
        [Row("/x//y")]
        [Row("/x/y/z/")]
        [ExpectedArgumentException]
        public void Parse_invalid_input_should_throw_exception(string input)
        {
            XmlPathRoot.Parse(input);
        }

        [Test]
        [Row("/")]
        [Row("/x/y/z")]
        [Row("/x/y/z:a")]
        public void Parse(string input)
        {
            var path = XmlPathRoot.Parse(input);
            Assert.AreEqual(input, path.ToString());
        }

        #endregion

        #region Strict paths building

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Element_with_negative_index_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(-1);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Attribute_with_negative_index_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Attribute(-1);
        }

        [Test]
        public void Formats_empty_strict_path()
        {
            var actual = XmlPathRoot.Strict.Empty.ToString();
            Assert.AreEqual("/", actual);
        }

        [Test]
        public void Formats_strict_path_with_one_element()
        {
            var actual = XmlPathRoot.Strict.Element(123).ToString();
            Assert.AreEqual("/123", actual);
        }

        [Test]
        public void Formats_complex_strict_path()
        {
            var actual = XmlPathRoot.Strict.Element(123).Element(456).Element(789).Attribute(666).ToString();
            Assert.AreEqual("/123/456/789:666", actual);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_non_root_element_to_declaration_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Declaration();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_attribute_to_declaration_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Attribute(123).Declaration();
        }

        [Test]
        public void Extends_root_to_declaration()
        {
            var actual = XmlPathRoot.Strict.Empty.Declaration().ToString();
            Assert.AreEqual("/-1", actual);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_root_to_attribute_should_throw_exception()
        {
            XmlPathRoot.Strict.Empty.Attribute(123);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_attribute_to_attribute_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Attribute(123).Attribute(123);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_declaration_to_attribute_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Declaration().Attribute(123);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_declaration_to_element_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Declaration().Element(123);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extends_declaration_to_declaration_should_throw_exception()
        {
            XmlPathRoot.Strict.Element(123).Declaration().Declaration();
        }

        #endregion

        #region Strict paths parsing

        [Test]
        [ExpectedArgumentNullException]
        public void Parse_strict_null_should_throw_exception()
        {
            XmlPathRoot.Strict.Parse(null);
        }

        [Test]
        [Row("")]
        [Row("blabla")]
        [Row("0")]
        [Row("/0/1:0/2")]
        [Row("/0//0")]
        [Row("/0/0/0/")]
        [Row("/0/x/0/")]
        [ExpectedArgumentException]
        public void Parsee_strict_invalid_input_should_throw_exception(string input)
        {
            XmlPathRoot.Parse(input);
        }

        [Test]
        [Row("/")]
        [Row("/0/0/0")]
        [Row("/0/0/0:0")]
        public void Parsee_strict(string input)
        {
            var path = XmlPathRoot.Parse(input);
            Assert.AreEqual(input, path.ToString());
        }

        #endregion
    }
}
