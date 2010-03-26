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
using MbUnit.Framework.ContractVerifiers;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(NodeAttribute))]
    public class NodeAttributeTest : DiffableTestBase
    {

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_index_should_throw_exception()
        {
            new NodeAttribute(-1, "name", "value", 1);
        }
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_name_should_throw_exception()
        {
            new NodeAttribute(0, null, "value", 1);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_value_should_throw_exception()
        {
            new NodeAttribute(0, "name", null, 1);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_invalid_count_should_throw_exception([Column(-1, 0, 5)] int invalidCount)
        {
            new NodeAttribute(5, "name", "value", invalidCount);
        }

        [Test]
        public void Constructs_ok()
        {
            var attribute = new NodeAttribute(123, "planet", "Saturn", 456);
            Assert.AreEqual(123, attribute.Index);
            Assert.AreEqual("planet", attribute.Name);
            Assert.AreEqual("Saturn", attribute.Value);
        }

        [Test]
        [Row("planet", Options.None, true)]
        [Row("planet", Options.IgnoreAttributesNameCase, true)]
        [Row("PLANET", Options.None, false)]
        [Row("PLANET", Options.IgnoreAttributesNameCase, true)]
        [Row("star", Options.None, false)]
        [Row("star", Options.IgnoreAttributesNameCase, false)]
        [Row("STAR", Options.None, false)]
        [Row("STAR", Options.IgnoreAttributesNameCase, false)]
        public void AreNamesEqual(string otherName, Options options, bool expected)
        {
            var attribute = new NodeAttribute(123, "planet", "Saturn", 456);
            bool actual = attribute.AreNamesEqual(otherName, options);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Row("Saturn", Options.None, true)]
        [Row("Saturn", Options.IgnoreAttributesValueCase, true)]
        [Row("SATURN", Options.None, false)]
        [Row("SATURN", Options.IgnoreAttributesValueCase, true)]
        [Row("Uranus", Options.None, false)]
        [Row("Uranus", Options.IgnoreAttributesValueCase, false)]
        [Row("URANUS", Options.None, false)]
        [Row("URANUS", Options.IgnoreAttributesValueCase, false)]
        public void AreValuesEqual(string othervalue, Options options, bool expected)
        {
            var attribute = new NodeAttribute(123, "planet", "Saturn", 456);
            bool actual = attribute.AreValuesEqual(othervalue, options);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            actual.Diff(null, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(123, "planet", "Saturn", 456);
            actual.Diff(expected, null, XmlOptions.Strict.Value);
        }

        [Test]
        public void Diff_equal_attributes_with_same_index()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(123, "planet", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_attributes_with_different_index()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(456, "planet", "Saturn", 789);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_attributes_ignoring_name_case()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(123, "PLANET", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesNameCase);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_attributes_with_name_differing_by_case()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(123, "PLANET", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.UnexpectedAttribute, XmlPathRoot.Strict.Element(0).Attribute(123), DiffTargets.Actual));
        }

        [Test]
        public void Diff_attributes_with_different_name()
        {
            var actual = new NodeAttribute(123, "planet", "Saturn", 456);
            var expected = new NodeAttribute(123, "orb", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.UnexpectedAttribute, XmlPathRoot.Strict.Element(0).Attribute(123), DiffTargets.Actual));
        }

        [Test]
        public void Diff_attributes_with_value_differing_by_case()
        {
            var actual = new NodeAttribute(123, "planet", "sAtUrN", 456);
            var expected = new NodeAttribute(123, "planet", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.MismatchedAttribute, XmlPathRoot.Strict.Element(0).Attribute(123), DiffTargets.Both));
        }

        [Test]
        public void Diff_attributes_ignoring_value_case()
        {
            var actual = new NodeAttribute(123, "planet", "sAtUrN", 456);
            var expected = new NodeAttribute(123, "planet", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), Options.IgnoreAttributesValueCase);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_attributes_with_different_value()
        {
            var actual = new NodeAttribute(123, "planet", "Jupiter", 456);
            var expected = new NodeAttribute(123, "planet", "Saturn", 456);
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Element(0), XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(DiffType.MismatchedAttribute, XmlPathRoot.Strict.Element(0).Attribute(123), DiffTargets.Both));
        }
    }
}
