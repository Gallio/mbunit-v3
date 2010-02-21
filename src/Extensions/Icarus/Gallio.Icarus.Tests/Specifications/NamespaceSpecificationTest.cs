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
