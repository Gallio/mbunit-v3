using Gallio.Model;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class FilterNode : TestTreeNode
    {
        public FilterNode(TestStatus testStatus)
            : base(testStatus.ToString(), testStatus.ToString())
        {
            switch (testStatus)
            {
                case TestStatus.Passed:
                    NodeTypeIcon = Properties.Resources.FilterPassed;
                    break;
                case TestStatus.Failed:
                    NodeTypeIcon = Properties.Resources.FilterFailed;
                    break;
                case TestStatus.Skipped:
                    NodeTypeIcon = Properties.Resources.FilterSkipped;
                    break;
            }
            TestStatus = testStatus;
        }
    }
}
