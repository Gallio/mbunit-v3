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
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(Comment))]
    public class CommentTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_value_should_throw_exception()
        {
            new Comment(null);
        }

        [Test]
        public void Constructs_empty_element()
        {
            var comment = new Comment(" Jack Burton ");
            Assert.IsFalse(comment.IsNull);
            Assert.AreSame(Null.Instance, comment.Child);
            Assert.AreSame(AttributeCollection.Empty, comment.Attributes);
            Assert.IsEmpty(comment.Name);
            Assert.AreEqual(" Jack Burton ", comment.Value);
            Assert.AreEqual("<!-- Jack Burton -->", comment.ToXml());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_expected_parameter_should_throw_exception()
        {
            var comment = new Comment(" Jack Burton ");
            comment.Diff(null, XmlPath.Empty, XmlOptions.Default.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Diff_with_null_pathshould_throw_exception()
        {
            var comment1 = new Comment("Chapi");
            var comment2 = new Comment("Chapo");
            comment1.Diff(comment2, null, XmlOptions.Default.Value);
        }

        [Test]
        public void Diff_inequal_comments()
        {
            var comment1 = new Comment("Chapi");
            var comment2 = new Comment("Chapo");
            DiffSet diffSet = comment1.Diff(comment2, XmlPath.Empty, XmlOptions.Strict.Value);
            AssertDiff(diffSet, new Diff(String.Empty, "Unexpected comment found.", "Chapo", "Chapi"));
        }

        [Test]
        public void Diff_while_ignoring_comments()
        {
            var comment1 = new Comment("Chapi");
            var comment2 = new Comment("Chapo");
            DiffSet diffSet = comment1.Diff(comment2, XmlPath.Empty, Options.IgnoreComments);
            Assert.IsTrue(diffSet.IsEmpty);
        }

        [Test]
        public void Diff_equal_comments()
        {
            var comment1 = new Comment("The same comment...");
            var comment2 = new Comment("The same comment...");
            DiffSet diffSet = comment1.Diff(comment2, XmlPath.Empty, XmlOptions.Strict.Value);
            Assert.IsTrue(diffSet.IsEmpty);
        }
    }
}
