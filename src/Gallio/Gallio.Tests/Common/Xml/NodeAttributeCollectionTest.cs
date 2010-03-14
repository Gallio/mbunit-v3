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

using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using Gallio.Tests.Common.Xml.Diffing;
using MbUnit.Framework;
using Gallio.Common.Xml;
using Gallio.Common;
using System.Collections.Generic;
using System;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(NodeAttributeCollection))]
    public class NodeAttributeCollectionTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_initializer_should_throw_exception()
        {
            new NodeAttributeCollection(null);
        }

        [Test]
        public void Default_empty()
        {
            var collection = NodeAttributeCollection.Empty;
            Assert.IsEmpty(collection);
            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void Constructs_ok()
        {
            var attribute1 = new Gallio.Common.Xml.NodeAttribute(123, "name1", "value1", 999);
            var attribute2 = new Gallio.Common.Xml.NodeAttribute(456, "name2", "value2", 999);
            var attribute3 = new Gallio.Common.Xml.NodeAttribute(789, "name3", "value3", 999);
            var array = new[] { attribute1, attribute2, attribute3 };
            var collection = new NodeAttributeCollection(array);
            Assert.AreEqual(3, collection.Count);
            Assert.AreElementsSame(array, collection);
        }

        private static NodeAttributeCollection MakeStubCollection(params string[] namesValues)
        {
            var list = new List<Gallio.Common.Xml.NodeAttribute>();

            for (int i = 0; i < namesValues.Length / 2; i++)
            {
                list.Add(new Gallio.Common.Xml.NodeAttribute(i, namesValues[2 * i], namesValues[2 * i + 1], namesValues.Length / 2));
            }

            return new NodeAttributeCollection(list);
        }

        [Test]
        public void Contains_with_null_name_should_throw_exception()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            Assert.Throws<ArgumentNullException>(() => attributes.Contains(null, null, Options.None));
        }

        [Test]
        public void Contains_yes()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name2", null, Options.None);
            Assert.IsTrue(found);
        }

        [Test]
        public void Contains_case_insensitive_yes()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("NAME2", null, Options.IgnoreAttributesNameCase);
            Assert.IsTrue(found);
        }

        [Test]
        public void Contains_no()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name123", null, Options.None);
            Assert.IsFalse(found);
        }

        [Test]
        public void Contains_case_senstive_no()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("NAME123", null, Options.None);
            Assert.IsFalse(found);
        }

        [Test]
        public void Contains_with_case_sensitive_value_yes()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name2", "value2", Options.None);
            Assert.IsTrue(found);
        }

        [Test]
        public void Contains_with_case_insensitive_value_yes()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name2", "VALUE2", Options.IgnoreAttributesValueCase);
            Assert.IsTrue(found);
        }

        [Test]
        public void Contains_with_case_sensitive_value_no()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name2", "VALUE2", Options.None);
            Assert.IsFalse(found);
        }

        [Test]
        public void Contains_with_case_insensitive_value_no()
        {
            var attributes = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            bool found = attributes.Contains("name2", "value3", Options.None);
            Assert.IsFalse(found);
        }

        [Test]
        public void Contains_value_among_several()
        {
            var attributes = MakeStubCollection("name", "value1", "name", "value2", "name", "value3");
            bool found = attributes.Contains("name", "value3", Options.None);
            Assert.IsTrue(found);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
            var collection = NodeAttributeCollection.Empty;
            collection.Diff(null, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var collection = NodeAttributeCollection.Empty;
            collection.Diff(NodeAttributeCollection.Empty, null, XmlOptions.Strict.Value);
        }

        #region Diffing ordered attributes

        [Test]
        public void Diff_equal_collections()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            Assert.IsEmpty(diffSet);
        }

        [Test]
        public void Diff_collections_with_missing_attribute_at_the_end()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "value2");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { new Diff("Missing attribute.", XmlPathRoot.Strict.Element(0).Attribute(2), DiffTargets.Expected) });
        }

        [Test]
        public void Diff_collections_with_missing_attribute_in_the_middle()
        {
            var actual = MakeStubCollection("name1", "value1", "name3", "value3");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute found.", XmlPathRoot.Strict.Element(0).Attribute(1), DiffTargets.Actual) });
        }

        [Test]
        public void Diff_collections_with_exceeding_attribute_at_the_end()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute found.", XmlPathRoot.Strict.Element(0).Attribute(2), DiffTargets.Actual) });
        }

        [Test]
        public void Diff_collections_with_exceeding_attribute_in_the_middle()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            var expected = MakeStubCollection("name1", "value1", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute found.", XmlPathRoot.Strict.Element(0).Attribute(1), DiffTargets.Actual) });
        }

        [Test]
        public void Diff_collections_with_one_unexpected_value()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "ERROR!", "name3", "value3");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute value found.", XmlPathRoot.Strict.Element(0).Attribute(1), DiffTargets.Both) });
        }

        [Test]
        public void Diff_collections_with_several_unexpected_values()
        {
            var actual = MakeStubCollection("name1", "ERROR1!", "name2", "value2", "name3", "ERROR3!");
            var expected = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diffSet, new[] { 
                new Diff("Unexpected attribute value found.", XmlPathRoot.Strict.Element(0).Attribute(0), DiffTargets.Both),
                new Diff("Unexpected attribute value found.", XmlPathRoot.Strict.Element(0).Attribute(2), DiffTargets.Both) });
        }

        #endregion

        #region Diffing unordered attributes

        [Test]
        public void Diff_equal_unordered_collections()
        {
            var actual = MakeStubCollection("name1", "value1", "name2", "value2", "name3", "value3");
            var expected = MakeStubCollection("name2", "value2", "name3", "value3", "name1", "value1");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesOrder);
            Assert.IsEmpty(diffSet);
        }

        [Test]
        public void Diff_equal_unordered_collections_with_missing_attribute()
        {
            var actual = MakeStubCollection("name1", "value1", "name3", "value3");
            var expected = MakeStubCollection("name2", "value2", "name3", "value3", "name1", "value1");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesOrder);
            AssertDiff(diffSet, new[] { new Diff("Missing attribute.", XmlPathRoot.Strict.Element(0).Attribute(0), DiffTargets.Expected) });
        }

        [Test]
        public void Diff_equal_unordered_collections_with_excess_attribute()
        {
            var actual = MakeStubCollection("name1", "value1", "name3", "value3", "name2", "value2");
            var expected = MakeStubCollection("name2", "value2", "name1", "value1");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesOrder);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute found.", XmlPathRoot.Strict.Element(0).Attribute(1), DiffTargets.Actual) });
        }

        [Test]
        public void Diff_equal_unordered_collections_with_unexpected_attribute_value()
        {
            var actual = MakeStubCollection("name1", "value1", "name3", "ERROR!", "name2", "value2");
            var expected = MakeStubCollection("name2", "value2", "name1", "value1", "name3", "value3");
            DiffSet diffSet = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesOrder);
            AssertDiff(diffSet, new[] { new Diff("Unexpected attribute value found.", XmlPathRoot.Strict.Element(0).Attribute(1), DiffTargets.Both) });
        }

        #endregion
    }
}
