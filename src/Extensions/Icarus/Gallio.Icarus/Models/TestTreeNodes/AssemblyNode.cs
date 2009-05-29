using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class AssemblyNode : TestDataNode
    {
        public AssemblyNode(TestData testData)
            : base(testData)
        {
            TestKind = TestKinds.Assembly;
        }
    }
}
