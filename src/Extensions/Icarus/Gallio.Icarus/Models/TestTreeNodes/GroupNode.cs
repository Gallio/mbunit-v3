using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class GroupNode : TestDataNode
    {
        public GroupNode(TestData testData)
            : base(testData)
        {
            NodeTypeIcon = Properties.Resources.Group;
            TestKind = TestKinds.Group;
        }
    }
}
