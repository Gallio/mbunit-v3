using Gallio.Icarus.Events;

namespace Gallio.Icarus.Models
{
    public class SortTreeEvent : Event
    {
        public SortOrder SortOrder { get; private set; }

        public SortTreeEvent(SortOrder sortOrder)
        {
            SortOrder = sortOrder;
        }
    }
}