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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(Attribute))]
    public class AttributeTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_name_should_throw_exception()
        {
            new Attribute(null, "value");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_value_should_throw_exception()
        {
            new Attribute("name", null);
        }

        [Test]
        public void Constructs_ok()
        {
            var attribute = new Attribute("planet", "Saturn");
            Assert.AreEqual("planet", attribute.Name);
            Assert.AreEqual("Saturn", attribute.Value);
            Assert.AreEqual("planet=\"Saturn\"", attribute.ToXml());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
             var actual = new Attribute("planet", "Saturn");
             actual.Diff(null, XmlPath.Empty, XmlOptions.Strict.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var actual = new Attribute("planet", "Saturn");
            var expected = new Attribute("planet", "Saturn");
            actual.Diff(expected, null, XmlOptions.Strict.Value);
        }

        [Test]
        public void Diff_equal_attributes()
        {
            var actual = new Attribute("planet", "Saturn");
            var expected = new Attribute("planet", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_attributes_ignoring_name_case()
        {
            var actual = new Attribute("planet", "Saturn");
            var expected = new Attribute("PLANET", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, Options.IgnoreAttributesNameCase);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_attributes_with_name_differing_by_case()
        {
            var actual = new Attribute("planet", "Saturn");
            var expected = new Attribute("PLANET", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(string.Empty, "Unexpected attribute found.", "PLANET", "planet"));
        }

        [Test]
        public void Diff_attributes_with_different_name()
        {
            var actual = new Attribute("planet", "Saturn");
            var expected = new Attribute("orb", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(string.Empty, "Unexpected attribute found.", "orb", "planet"));
        }

        [Test]
        public void Diff_attributes_with_value_differing_by_case()
        {
            var actual = new Attribute("planet", "sAtUrN");
            var expected = new Attribute("planet", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(string.Empty, "Unexpected attribute value found.", "Saturn", "sAtUrN"));
        }

        [Test]
        public void Diff_attributes_ignoring_value_case()
        {
            var actual = new Attribute("planet", "sAtUrN");
            var expected = new Attribute("planet", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, Options.IgnoreAttributesValueCase);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_attributes_with_different_value()
        {
            var actual = new Attribute("planet", "Jupiter");
            var expected = new Attribute("planet", "Saturn");
            var diff = actual.Diff(expected, XmlPath.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff(string.Empty, "Unexpected attribute value found.", "Saturn", "Jupiter"));
        }
    }
}
