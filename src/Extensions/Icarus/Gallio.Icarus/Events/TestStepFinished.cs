namespace Gallio.Icarus.Events
{
    public class TestStepFinished : Event
    {
        public string TestId { get; private set; }

        public TestStepFinished(string testId)
        {
            TestId = testId;
        }
    }
}