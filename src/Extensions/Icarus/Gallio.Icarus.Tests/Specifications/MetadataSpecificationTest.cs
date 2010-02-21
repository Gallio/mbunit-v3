using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.Specifications;
using Gallio.Model.Schema;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Specifications
{
    public class MetadataSpecificationTest
    {
        [Test]
        public void Metadata_spec_should_match_on_value()
        {
            const string metadataKey = "test";
            const string metadataValue = "some node text";
            var specification = new MetadataSpecification(metadataKey, metadataValue);
            var testData = new TestData("id", "name", "fullName");
            testData.Metadata.Add(metadataKey, metadataValue);

            var matches = specification.Matches(new TestDataNode(testData));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Name_spec_should_match_on_partial_text()
        {
            const string metadataKey = "test";
            var specification = new MetadataSpecification(metadataKey, "node");
            var testData = new TestData("id", "name", "fullName");
            testData.Metadata.Add(metadataKey, "some node text");

            var matches = specification.Matches(new TestDataNode(testData));

            Assert.IsTrue(matches);
        }

        [Test]
        public void Name_spec_should_not_match_if_no_metadata_matches()
        {
            var specification = new MetadataSpecification("test", "node");

            var matches = specification.Matches(new TestDataNode(new TestData("id", 
                "name", "fullName")));

            Assert.IsFalse(matches);
        }
    }
}
