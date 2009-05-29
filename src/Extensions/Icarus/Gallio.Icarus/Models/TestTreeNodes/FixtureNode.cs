using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class FixtureNode : TestDataNode
    {
        public FixtureNode(TestData testData)
            : base(testData)
        {
            TestKind = TestKinds.Fixture;
        }
    }
}
