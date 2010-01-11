using System.Collections.Generic;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Events
{
    public class TestSelectionChanged : Event
    {
        public IList<TestTreeNode> Nodes { get; private set; }

        public TestSelectionChanged(IEnumerable<TestTreeNode> nodes)
        {
            Nodes = new List<TestTreeNode>(nodes);
        }
    }
}