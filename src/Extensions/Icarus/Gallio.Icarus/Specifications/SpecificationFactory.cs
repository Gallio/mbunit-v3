using Gallio.Icarus.Models;

namespace Gallio.Icarus.Specifications
{
    public class SpecificationFactory : ISpecificationFactory
    {
        public ISpecification<TestTreeNode> Create(string metadataType, string searchText)
        {
            switch (metadataType)
            {
                case "Name":
                    return new NameSpecification(searchText);
                case "Namespace":
                    return new NamespaceSpecification(searchText);
                default:
                    return new MetadataSpecification(metadataType, searchText);
            }
        }
    }
}