using Gallio.Common.Reflection;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class TestDataNode : TestTreeNode
    {
        public TestDataNode(TestData testData)
            : base(testData.Id, testData.Name)
        {
            SourceCodeAvailable = (testData.CodeLocation != CodeLocation.Unknown);
            IsTest = testData.IsTestCase;
        }
    }
}
