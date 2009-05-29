using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class RootNode : TestDataNode
    {
        public RootNode(TestData testData)
            : base(testData)
        {
            TestKind = TestKinds.Root;
        }
    }
}
