using Gallio.Icarus.Models;

namespace Gallio.Icarus.Specifications
{
    public class NameSpecification : Specification<TestTreeNode>
    {
        public NameSpecification(string name)
        {
            Name = name;
        }

        public string Name
        {
            get; private set;
        }

        public override bool Matches(TestTreeNode item)
        {
            return CaseInsensitiveContains(item.Text, Name);
        }
    }
}
