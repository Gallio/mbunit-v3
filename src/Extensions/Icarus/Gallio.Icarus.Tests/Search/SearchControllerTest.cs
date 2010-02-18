using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Search;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Search
{
    public class SearchControllerTest
    {
        private Controller searchController;
        private IEventAggregator eventAggregator;
        private IOptionsController optionsController;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(oc => oc.GetCurrentSettings()).Return(new Settings());
            searchController = new Controller(new Icarus.Search.Model(), eventAggregator, 
                optionsController);
        }

        [Test]
        public void Search_should_fire_an_event()
        {
            const string searchText = "searchText";
            const string metadataType = "metadataType";
            var testTreeNode = new TestTreeNode("name", searchText);

            searchController.Search(searchText, metadataType);
            
            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<FilterTreeEvent>.Matches(fte => 
                fte.Specification.Matches(testTreeNode))));
        }
    }
}
