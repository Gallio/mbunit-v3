using Gallio.Icarus.Events;
using Gallio.Model;

namespace Gallio.Icarus.Models
{
    public class FilterTestStatusEvent : Event
    {
        public TestStatus TestStatus { get; private set; }

        public FilterTestStatusEvent(TestStatus testStatus)
        {
            TestStatus = testStatus;
        }
    }
}