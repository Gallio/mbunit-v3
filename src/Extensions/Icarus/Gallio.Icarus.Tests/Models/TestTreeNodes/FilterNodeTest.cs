using System.Windows.Forms;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models.TestTreeNodes
{
    [Category("TestTreeNodes"), TestsOn(typeof(FilterNode))]
    public class FilterNodeTest
    {
        [Test]
        public void Name_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Passed;
            
            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus.ToString(), filterNode.Name);
        }

        [Test]
        public void Text_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Failed;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus.ToString(), filterNode.Text);
        }

        [Test]
        public void TestStatus_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Skipped;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus, filterNode.TestStatus);
        }

        [Test]
        public void CheckState_should_be_checked()
        {
            const TestStatus testStatus = TestStatus.Inconclusive;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(CheckState.Checked, filterNode.CheckState);
        }
    }
}
