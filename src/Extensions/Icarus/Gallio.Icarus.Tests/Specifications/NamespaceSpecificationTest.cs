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

using Gallio.Common.Reflection;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.Specifications;
using Gallio.Model.Schema;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Specifications
{
    public class NamespaceSpecificationTest
    {
        [Test]
        public void Namespace_spec_should_match_on_namespace()
        {
            const string testNamespace = "some.test.namespace";
            var specification = new NamespaceSpecification(testNamespace);
            var codeReference = new CodeReference("", testNamespace, "", "", "");
            var testData = new TestData("id", "name", "fullName")
            {
                CodeReference = codeReference
            };

            var matches = specification.Matches(new TestDataNode(testData));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Namespace_spec_should_match_on_partial_namespace()
        {
            var specification = new NamespaceSpecification("test");
            var codeReference = new CodeReference("", "some.test.namespace", "", "", "");
            var testData = new TestData("id", "name", "fullName")
            {
                CodeReference = codeReference
            };

            var matches = specification.Matches(new TestDataNode(testData));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Namespace_spec_should_not_match_if_namespace_is_different()
        {
            var specification = new NamespaceSpecification("wahwahwah");
            var codeReference = new CodeReference("", "some.test.namespace", "", "", "");
            var testData = new TestData("id", "name", "fullName")
            {
                CodeReference = codeReference
            };

            var matches = specification.Matches(new TestDataNode(testData));

            Assert.IsFalse(matches);
        }
    }
}
