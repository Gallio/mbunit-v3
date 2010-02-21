using Gallio.Icarus.Models;

namespace Gallio.Icarus.Specifications
{
    public interface ISpecificationFactory
    {
        ISpecification<TestTreeNode> Create(string metadataType, string searchText);
    }
}