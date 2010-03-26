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
using Gallio.Common.Collections;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using Gallio.Tests.Common.Xml.Diffing;
using MbUnit.Framework;
using Gallio.Common.Xml;
using Rhino.Mocks;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(NodeElement))]
    public class NodeElementTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_index_should_throw_exception()
        {
            new NodeElement(-1, 123, "name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
        }
        
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_name_should_throw_exception()
        {
            new NodeElement(0, 123, null, NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_attributes_should_throw_exception()
        {
            new NodeElement(0, 123, "name", null, EmptyArray<INode>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_child_should_throw_exception()
        {
            new NodeElement(0, 123, "name", NodeAttributeCollection.Empty, null);
        }


        [Test]
        public void Constructs_empty_element([Column(0, 123)] int index)
        {
            var element = new NodeElement(index, 123, "Planet", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            Assert.AreEqual(index, element.Index);
            Assert.IsEmpty(element.Children);
            Assert.IsEmpty(element.Attributes);
            Assert.AreEqual("Planet", element.Name);
        }

        [Test]
        public void Constructs_non_empty_element()
        {
            var attribute1 = new Gallio.Common.Xml.NodeAttribute(0, "diameter", "4878 km", 2);
            var attribute2 = new Gallio.Common.Xml.NodeAttribute(1, "revolution", "58.6 d", 2);
            var mockChild1 = MockRepository.GenerateStub<INode>();
            var mockChild2 = MockRepository.GenerateStub<INode>();
            var element = new NodeElement(0, 123, "Planet", new[] { attribute1, attribute2 }, new[] { mockChild1, mockChild2 });
            Assert.AreElementsSame(new[] { mockChild1, mockChild2 }, element.Children);
            Assert.AreElementsSame(new[] { attribute1, attribute2 }, element.Attributes);
            Assert.AreEqual("Planet", element.Name);
        }


        [Test]
        [Row("planet", Options.None, true)]
        [Row("planet", Options.IgnoreElementsNameCase, true)]
        [Row("PLANET", Options.None, false)]
        [Row("PLANET", Options.IgnoreElementsNameCase, true)]
        [Row("star", Options.None, false)]
        [Row("star", Options.IgnoreElementsNameCase, false)]
        [Row("STAR", Options.None, false)]
        [Row("STAR", Options.IgnoreElementsNameCase, false)]
        public void AreNamesEqual(string otherName, Options options, bool expected)
        {
            var element = new NodeElement(0, 123, "planet", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            bool actual = element.AreNamesEqual(otherName, options);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
            var actual = new NodeElement(123, 123, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            actual.Diff(null, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var actual = new NodeElement(123, 123, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(123, 123, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            actual.Diff(expected, null, XmlOptions.Strict.Value);
        }

        [Test]
        public void Diff_equal_with_different_index()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(456, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_with_same_index()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_with_item_differing_by_name()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(123, 456, "SomeOtherName", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.MismatchedElement, XmlPathRoot.Strict.Empty.Element(123), DiffTargets.Both));
        }

        [Test]
        public void Diff_with_item_differing_by_name_case()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(123, 456, "NAME", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.MismatchedElement, XmlPathRoot.Strict.Empty.Element(123), DiffTargets.Both));
        }

        [Test]
        public void Diff_equal_with_item_differing_by_name_case()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = new NodeElement(123, 456, "NAME", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Custom.IgnoreElementsNameCase.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_with_items_differing_by_type()
        {
            var actual = new NodeElement(123, 456, "Name", NodeAttributeCollection.Empty, EmptyArray<INode>.Instance);
            var expected = MockRepository.GenerateStub<INode>();
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.UnexpectedElement, XmlPathRoot.Strict.Empty.Element(123), DiffTargets.Actual));
        }
    }
}
