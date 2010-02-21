using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Search;
using Gallio.Icarus.Specifications;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Search
{
    public class SearchControllerTest
    {
        private Controller searchController;
        private IEventAggregator eventAggregator;
        private IOptionsController optionsController;
        private ISpecificationFactory specificationFactory;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(oc => oc.GetCurrentSettings()).Return(new Settings());
            
            specificationFactory = MockRepository.GenerateStub<ISpecificationFactory>();
            
            searchController = new Controller(new Icarus.Search.Model(), eventAggregator, 
                optionsController, specificationFactory);
        }

        [Test]
        public void Search_should_create_an_appropriate_specification()
        {
            const string metadataType = "metadataType";
            const string searchText = "searchText";

            searchController.Search(metadataType, searchText);

            specificationFactory.AssertWasCalled(sf => sf.Create(metadataType, searchText));
        }

        [Test]
        public void Search_should_send_the_specification_to_the_event_aggregator()
        {
            var specification = MockRepository.GenerateStub<ISpecification<TestTreeNode>>();
            specificationFactory.Stub(sf => sf.Create(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(specification);

            searchController.Search("metadataType", "searchText");

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<FilterTreeEvent>.Matches(fte =>
                fte.Specification == specification)));
        }
    }
}
