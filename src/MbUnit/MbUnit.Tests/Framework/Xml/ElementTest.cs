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
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using MbUnit.Framework.Xml;

namespace MbUnit.Tests.Framework.Xml
{
    [TestFixture]
    public class ElementTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_child_should_throw_exception()
        {
            new Element(null, "name", "value", AttributeCollection.Empty);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_name_should_throw_exception()
        {
            new Element(Null.Instance, null, "value", AttributeCollection.Empty);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_value_should_throw_exception()
        {
            new Element(Null.Instance, "name", null, AttributeCollection.Empty);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_attributes_should_throw_exception()
        {
            new Element(Null.Instance, "name", "value", null);
        }

        [Test]
        public void Constructs_empty_element()
        {
            var element = new Element(Null.Instance, "Planet", String.Empty, AttributeCollection.Empty);
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(Null.Instance, element.Child);
            Assert.AreSame(AttributeCollection.Empty, element.Attributes);
            Assert.AreEqual("Planet", element.Name);
            Assert.IsEmpty(element.Value);
            Assert.AreEqual("<Planet/>", element.ToXml());
        }

        [Test]
        public void Constructs_value_element()
        {
            var element = new Element(Null.Instance, "Planet", "Mercury", AttributeCollection.Empty);
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(Null.Instance, element.Child);
            Assert.AreSame(AttributeCollection.Empty, element.Attributes);
            Assert.AreEqual("Planet", element.Name);
            Assert.AreEqual("Mercury", element.Value);
            Assert.AreEqual("<Planet>Mercury</Planet>", element.ToXml());
        }

        [Test]
        public void Constructs_empty_element_with_one_attribute()
        {
            var element = new Element(Null.Instance, "Planet", String.Empty, 
                new AttributeCollection(new[] { new MbUnit.Framework.Xml.Attribute("diameter", "4878 km") }));
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(Null.Instance, element.Child);
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual("Planet", element.Name);
            Assert.IsEmpty(element.Value);
            Assert.AreEqual("<Planet diameter=\"4878 km\"/>", element.ToXml());
        }

        [Test]
        public void Constructs_empty_element_with_several_attributes()
        {
            var element = new Element(Null.Instance, "Planet", String.Empty,
                new AttributeCollection(new[] { 
                    new MbUnit.Framework.Xml.Attribute("diameter", "4878 km"),
                    new MbUnit.Framework.Xml.Attribute("revolution", "58.6 d")
                }));
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(Null.Instance, element.Child);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("Planet", element.Name);
            Assert.IsEmpty(element.Value);
            Assert.AreEqual("<Planet diameter=\"4878 km\" revolution=\"58.6 d\"/>", element.ToXml());
        }

        [Test]
        public void Constructs_value_element_with_several_attributes()
        {
            var element = new Element(Null.Instance, "Planet", "Mercury",
                new AttributeCollection(new[] { 
                    new MbUnit.Framework.Xml.Attribute("diameter", "4878 km"),
                    new MbUnit.Framework.Xml.Attribute("revolution", "58.6 d")
                }));
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(Null.Instance, element.Child);
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual("Planet", element.Name);
            Assert.AreEqual("Mercury", element.Value);
            Assert.AreEqual("<Planet diameter=\"4878 km\" revolution=\"58.6 d\">Mercury</Planet>", element.ToXml());
        }

        [Test]
        public void Constructs_with_child_elements()
        {
            var element = new Element(new ElementCollection(new[] {
                    new Element(Null.Instance, "Planet", "Mercury", AttributeCollection.Empty),
                    new Element(Null.Instance, "Planet", "Venus", AttributeCollection.Empty)
                }), "Planets", String.Empty, AttributeCollection.Empty);
            Assert.IsFalse(element.IsNull);
            Assert.AreSame(AttributeCollection.Empty, element.Attributes);
            Assert.AreEqual("Planets", element.Name);
            Assert.IsEmpty(element.Value);
            Assert.AreEqual("<Planets><Planet>Mercury</Planet><Planet>Venus</Planet></Planets>", element.ToXml());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
            var actual = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            actual.Diff(null, Path.Empty, XmlEqualityOptions.Strict);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var actual = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            var expected = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            actual.Diff(expected, null, XmlEqualityOptions.Strict);
        }

        [Test]
        public void Diff_equal_value_element()
        {
            var actual = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            var expected = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            var diff = actual.Diff(expected, Path.Empty, XmlEqualityOptions.Strict);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_elements_differing_by_name()
        {
            var actual = new Element(Null.Instance, "Orb", "Sun", AttributeCollection.Empty);
            var expected = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            var diff = actual.Diff(expected, Path.Empty, XmlEqualityOptions.Strict);
            AssertDiff(diff, new Diff(string.Empty, "Unexpected element found.", "Star", "Orb"));
        }

        [Test]
        public void Diff_elements_differing_by_value()
        {
            var actual = new Element(Null.Instance, "Star", "Alpha Centauri", AttributeCollection.Empty);
            var expected = new Element(Null.Instance, "Star", "Sun", AttributeCollection.Empty);
            var diff = actual.Diff(expected, Path.Empty, XmlEqualityOptions.Strict);
            AssertDiff(diff, new Diff("<Star>", "Unexpected element value found.", "Sun", "Alpha Centauri"));
        }

        [Test]
        public void Diff_elements_differing_by_attribute()
        {
            var attributeActual = new MbUnit.Framework.Xml.Attribute("magnitude", "Look at up the sky!");
            var attributeExpected = new MbUnit.Framework.Xml.Attribute("magnitude", "4.85");
            var actual = new Element(Null.Instance, "Star", "Sun", new AttributeCollection(new[] { attributeActual }));
            var expected = new Element(Null.Instance, "Star", "Sun", new AttributeCollection(new[] { attributeExpected }));
            var diff = actual.Diff(expected, Path.Empty, XmlEqualityOptions.Strict);
            AssertDiff(diff, new Diff("<Star magnitude='...'>", "Unexpected attribute value found.", "4.85", "Look at up the sky!"));
        }

        [Test]
        public void Diff_elements_differing_by_child()
        {
            var childActual = new Element(Null.Instance, "Planets", "9", AttributeCollection.Empty);
            var childExpected = new Element(Null.Instance, "Planets", "8", AttributeCollection.Empty);
            var actual = new Element(childActual, "Star", "Sun", AttributeCollection.Empty);
            var expected = new Element(childExpected, "Star", "Sun", AttributeCollection.Empty);
            var diff = actual.Diff(expected, Path.Empty, XmlEqualityOptions.Strict);
            AssertDiff(diff, new Diff("<Star><Planets>", "Unexpected element value found.", "8", "9"));
        }
    }
}
