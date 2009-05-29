using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runtime;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class FrameworkNode : TestDataNode
    {
        public FrameworkNode(TestData testData) 
            : base(testData)
        {
            TestKind = TestKinds.Framework;
        }
    }
}
