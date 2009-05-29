using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class TestNode : TestDataNode
    {
        public TestNode(TestData testData)
            : base(testData)
        {
            NodeTypeIcon = Properties.Resources.Test;
            TestKind = TestKinds.Test;
        }
    }
}
