using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Search;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Search
{
    public class SearchControllerTest
    {
        [Test]
        public void Search_should_fire_an_event()
        {
            var eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            var searchController = new Controller(eventAggregator);
            const string searchText = "searchText";
            var testTreeNode = new TestTreeNode("name", searchText);

            searchController.Search(searchText);
            
            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<FilterTreeEvent>.Matches(fte => 
                fte.Specification.Matches(testTreeNode))));
        }
    }
}
