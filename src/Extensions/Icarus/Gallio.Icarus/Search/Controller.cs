using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Search
{
    public class Controller : IController, Handles<OptionsChanged>
    {
        private readonly IModel model;
        private readonly IEventAggregator eventAggregator;
        private readonly IOptionsController optionsController;

        public Controller(IModel model, IEventAggregator eventAggregator, 
            IOptionsController optionsController)
        {
            this.model = model;
            this.eventAggregator = eventAggregator;
            this.optionsController = optionsController;

            UpdateMetadataOptions();
        }

        public void Search(string searchText, string metadataType)
        {
            var treeSpec = new TreeNodeSpecification(searchText, metadataType);
            var filterTreeEvent = new FilterTreeEvent(treeSpec);
            eventAggregator.Send(filterTreeEvent);
        }

        public void Handle(OptionsChanged @event)
        {
            UpdateMetadataOptions();
        }

        private void UpdateMetadataOptions()
        {
            var settings = optionsController.GetCurrentSettings();
            model.Metadata.Value = new List<string>(settings.TreeViewCategories);
        }
    }
}