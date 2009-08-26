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
using System.Collections.Generic;

namespace MbUnit.Tests.Framework.Xml
{
    [TestFixture]
    public class ElementCollectionTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_initializer_should_throw_exception()
        {
            new ElementCollection(null);
        }

        [Test]
        public void Constructs_ok()
        {
            var element1 = new Element(Null.Instance, "Child1", "Value1", AttributeCollection.Empty);
            var element2 = new Element(Null.Instance, "Child2", "Value2", AttributeCollection.Empty);
            var element3 = new Element(Null.Instance, "Child3", "Value3", AttributeCollection.Empty);
            var array = new[] { element1, element2, element3 };
            var collection = new ElementCollection(array);
            Assert.AreEqual(3, collection.Count);
            Assert.IsFalse(collection.IsNull);
            Assert.AreElementsEqual(array, collection, new StructuralEqualityComparer<Element>
            {
                { x => x.Name },
                { x => x.Value }
            });
            Assert.AreEqual("<Child1>Value1</Child1><Child2>Value2</Child2><Child3>Value3</Child3>", collection.ToXml());
        }

        private ElementCollection MakeCollection(params string[] namesValues)
        {
            var list = new List<Element>();

            for (int i = 0; i < namesValues.Length / 2; i++)
            {
                list.Add(new Element(Null.Instance, namesValues[2 * i], namesValues[2 * i + 1], AttributeCollection.Empty));
            }

            return new ElementCollection(list);
        }

        #region Ordered attributes

        [Test]
        public void Diff_equal_collections()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            Assert.IsEmpty(diffSet);
        }

        [Test]
        public void Diff_collections_with_missing_element_at_the_end()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "Value2");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Missing element.", "Child3", String.Empty) });
        }

        [Test]
        public void Diff_collections_with_missing_element_in_the_middle()
        {
            var actual = MakeCollection("Child1", "Value1", "Child3", "Value3");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Unexpected element found.", "Child2", "Child3") });
        }

        [Test]
        public void Diff_collections_with_exceeding_element_at_the_end()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Unexpected element found.", String.Empty, "Child3") });
        }

        [Test]
        public void Diff_collections_with_exceeding_element_in_the_middle()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            var expected = MakeCollection("Child1", "Value1", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Unexpected element found.", "Child3", "Child2") });
        }

        [Test]
        public void Diff_collections_with_one_unexpected_value()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "ERROR!", "Child3", "Value3");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { new Diff("<Root><Child2>", "Unexpected element value found.", "Value2", "ERROR!") });
        }

        [Test]
        public void Diff_collections_with_several_unexpected_values()
        {
            var actual = MakeCollection("Child1", "ERROR1!", "Child2", "Value2", "Child3", "ERROR3!");
            var expected = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.Strict);
            AssertDiff(diffSet, new[] { 
                new Diff("<Root><Child1>", "Unexpected element value found.", "Value1", "ERROR1!"),
                new Diff("<Root><Child3>", "Unexpected element value found.", "Value3", "ERROR3!") });
        }

        #endregion

        #region Unordered attributes

        [Test]
        public void Diff_equal_unordered_collections()
        {
            var actual = MakeCollection("Child1", "Value1", "Child2", "Value2", "Child3", "Value3");
            var expected = MakeCollection("Child2", "Value2", "Child3", "Value3", "Child1", "Value1");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.IgnoreElementsOrder);
            Assert.IsEmpty(diffSet);
        }

        [Test]
        public void Diff_equal_unordered_collections_with_missing_element()
        {
            var actual = MakeCollection("Child1", "Value1", "Child3", "Value3");
            var expected = MakeCollection("Child2", "Value2", "Child3", "Value3", "Child1", "Value1");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.IgnoreElementsOrder);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Missing element.", "Child2", String.Empty) });
        }

        [Test]
        public void Diff_equal_unordered_collections_with_excess_element()
        {
            var actual = MakeCollection("Child1", "Value1", "Child3", "Value3", "Child2", "Value2");
            var expected = MakeCollection("Child2", "Value2", "Child1", "Value1");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.IgnoreElementsOrder);
            AssertDiff(diffSet, new[] { new Diff("<Root>", "Unexpected element found.", String.Empty, "Child3") });
        }

        [Test]
        public void Diff_equal_unordered_collections_with_unexpected_element_value()
        {
            var actual = MakeCollection("Child1", "Value1", "Child3", "ERROR!", "Child2", "Value2");
            var expected = MakeCollection("Child2", "Value2", "Child1", "Value1", "Child3", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.IgnoreElementsOrder);
            AssertDiff(diffSet, new[] { new Diff("<Root><Child3>", "Unexpected element value found.", "Value3", "ERROR!") });
        }

        [Test]
        public void Diff_equal_unordered_set_with_unexpected_element_value()
        {
            var actual = MakeCollection("Child", "Value1", "Child", "ERROR!", "Child", "Value2");
            var expected = MakeCollection("Child", "Value1", "Child", "Value2", "Child", "Value3");
            DiffSet diffSet = actual.Diff(expected, Path.Empty.Extend("Root"), XmlEqualityOptions.IgnoreElementsOrder);
            AssertDiff(diffSet, new[] { new Diff("<Root><Child>", "Unexpected element value found.", "Value3", "ERROR!") });
        }

        #endregion
    }
}
