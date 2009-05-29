using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Helpers
{
    internal class TestTreeNodeFactory
    {
        public static TestTreeNode CreateNode(TestData testData)
        {
            string testKind = testData.Metadata.GetValue(MetadataKeys.TestKind);
            TestTreeNode testTreeNode;

            switch (testKind)
            {
                case TestKinds.Assembly:
                    testTreeNode = new AssemblyNode(testData);
                    break;

                case TestKinds.Fixture:
                    testTreeNode = new FixtureNode(testData);
                    break;

                case TestKinds.Test:
                    testTreeNode = new TestNode(testData);
                    break;

                case TestKinds.Group:
                    testTreeNode = new GroupNode(testData);
                    break;

                case TestKinds.Framework:
                    testTreeNode = new FrameworkNode(testData);
                    break;

                case TestKinds.Root:
                    testTreeNode = new RootNode(testData);
                    break;

                default:
                    testTreeNode = new TestDataNode(testData) 
                        { TestKind = testKind };
                    break;
            }

            return testTreeNode;
        }
    }
}
