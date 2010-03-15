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
    [TestsOn(typeof(NodeContent))]
    public class NodeContentTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_index_should_throw_exception()
        {
            new NodeContent(-1, 123, "blah blah");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_value_should_throw_exception()
        {
            new NodeContent(0, 123, null);
        }

        [Test]
        public void Constructs_ok([Column(0, 123)] int index)
        {
            var comment = new NodeContent(index, 123, " Jack Burton ");
            Assert.AreEqual(index, comment.Index);
            Assert.IsEmpty(comment.Children);
            Assert.AreEqual(" Jack Burton ", comment.Text);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_value_should_throw_exception()
        {
            var actual = new NodeContent(123, 123, "Text");
            actual.Diff(null, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_path_should_throw_exception()
        {
            var actual = new NodeContent(123, 123, "Text");
            var expected = new NodeContent(123, 123, "Text");
            actual.Diff(expected, null, XmlOptions.Strict.Value);
        }

        [Test]
        public void Diff_equal_with_different_index()
        {
            var actual = new NodeContent(123, 123, "Text");
            var expected = new NodeContent(456, 123, "Text");
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_with_same_index()
        {
            var actual = new NodeContent(123, 123, "Text");
            var expected = new NodeContent(123, 123, "Text");
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_equal_ignoring_name_case()
        {
            var actual = new NodeContent(123, 123, "Text");
            var expected = new NodeContent(123, 123, "TEXT");
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, Options.IgnoreElementsValueCase);
            Assert.IsTrue(diff.IsEmpty);
        }

        [Test]
        public void Diff_with_value_differing_by_case()
        {
            var actual = new NodeContent(123, 123, "Text");
            var expected = new NodeContent(123, 123, "TEXT");
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff("Unexpected text content found.", XmlPathRoot.Strict.Empty.Element(123), DiffTargets.Both));
        }

        [Test]
        public void Diff_with_items_differing_by_type()
        {
            var actual = new NodeContent(123, 123, "TEXT");
            var expected = MockRepository.GenerateStub<INode>();
            var diff = actual.Diff(expected, XmlPathRoot.Strict.Empty, XmlOptions.Strict.Value);
            AssertDiff(diff, new Diff("Unexpected text content found.", XmlPathRoot.Strict.Empty.Element(123), DiffTargets.Actual));
        }
    }
}
