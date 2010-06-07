using System.Collections.Generic;

namespace Gallio.Icarus.Projects
{
    public interface IUserOptionsController
    {
        IEnumerable<string> CollapsedNodes { get; }
        string TreeViewCategory { get; }
        void SetCollapsedNodes(IEnumerable<string> collapsedNodes);
    }
}