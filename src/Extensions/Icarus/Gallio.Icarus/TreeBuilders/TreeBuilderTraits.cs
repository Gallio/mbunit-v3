using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.TreeBuilders
{
    public class TreeBuilderTraits : Traits
    {
        public int Priority { get; private set; }

        public TreeBuilderTraits(int priority)
        {
            Priority = priority;
        }
    }
}