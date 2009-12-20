using Gallio.Icarus.Events;
using Gallio.Icarus.Specifications;

namespace Gallio.Icarus.Models
{
    public class FilterTreeEvent : Event
    {
        public ISpecification<TestTreeNode> Specification
        {
            get; private set;
        }

        public FilterTreeEvent(ISpecification<TestTreeNode> specification)
        {
            Specification = specification;
        }
    }
}