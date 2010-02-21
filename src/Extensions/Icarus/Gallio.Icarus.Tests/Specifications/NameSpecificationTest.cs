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
