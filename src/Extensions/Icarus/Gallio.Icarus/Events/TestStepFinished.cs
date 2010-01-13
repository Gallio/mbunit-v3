using Gallio.Model;

namespace Gallio.Icarus.Events
{
    public class TestStepFinished : Event
    {
        public string TestId { get; private set; }
        public TestStatus TestStatus { get; private set; }

        public TestStepFinished(string testId, TestStatus testStatus)
        {
            TestId = testId;
            TestStatus = testStatus;
        }
    }
}