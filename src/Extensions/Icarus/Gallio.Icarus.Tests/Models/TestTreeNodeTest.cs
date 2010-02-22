using Gallio.Icarus.Models;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models
{
    public class TestTreeNodeTest
    {
        [Test]
        public void When_a_nodes_test_status_is_updated_its_parent_should_be_too()
        {
            var parent = new TestTreeNode("parent", "parent");
            var child1 = new TestTreeNode("child1", "child1");
            var child2 = new TestTreeNode("child2", "child2");
            parent.Nodes.Add(child1);
            parent.Nodes.Add(child2);

            child1.TestStatus = TestStatus.Passed;

            Assert.AreEqual(TestStatus.Passed, parent.TestStatus);
        }
    }
}
