using Gallio.Icarus.Events;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Search
{
    public class Controller : IController
    {
        private readonly IEventAggregator eventAggregator;

        public Controller(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Search(string searchText)
        {
            var treeSpec = new TreeNodeSpecification(searchText);
            var filterTreeEvent = new FilterTreeEvent(treeSpec);
            eventAggregator.Send(filterTreeEvent);
        }
    }
}