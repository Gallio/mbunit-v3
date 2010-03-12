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

using Gallio.Icarus.Models;
using Gallio.Icarus.Specifications;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Specifications
{
    public class NameSpecificationTest
    {
        [Test]
        public void Name_spec_should_match_on_node_text()
        {
            const string text = "some node text";
            var specification = new NameSpecification(text);

            var matches = specification.Matches(new TestTreeNode("id", text));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Name_spec_should_match_on_partial_text()
        {
            var specification = new NameSpecification("node");

            var matches = specification.Matches(new TestTreeNode("id", "some node text"));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Namespace_spec_should_not_match_if_namespace_is_different()
        {
            var specification = new NameSpecification("wahwahwah");

            var matches = specification.Matches(new TestTreeNode("id", "some node text"));

            Assert.IsFalse(matches);
        }
    }
}
