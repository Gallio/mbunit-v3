using Gallio.Icarus.Models;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Specifications
{
    public class MetadataSpecification : Specification<TestTreeNode>
    {
        public string MetadataKey { get; private set; }

        public string MetadataValue { get; private set; }

        public MetadataSpecification(string metadataKey, string metadataValue)
        {
            MetadataKey = metadataKey;
            MetadataValue = metadataValue;
        }

        public override bool Matches(TestTreeNode item)
        {
            var testDescriptor = item as ITestDescriptor;

            if (testDescriptor == null)
                return false;

            return Matches(testDescriptor);
        }

        private bool Matches(ITestDescriptor testDescriptor)
        {
            var metadataList = testDescriptor.Metadata[MetadataKey];

            foreach (var metadata in metadataList)
            {
                if (CaseInsensitiveContains(metadata, MetadataValue))
                    return true;
            }
            
            return false;
        }
    }
}
