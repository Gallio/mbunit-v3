// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Specifications;
using Gallio.UI.Events;

namespace Gallio.Icarus.Search
{
    public class Controller : IController, Handles<OptionsChanged>
    {
        private readonly IModel model;
        private readonly IEventAggregator eventAggregator;
        private readonly IOptionsController optionsController;
        private readonly ISpecificationFactory specificationFactory;

        public Controller(IModel model, IEventAggregator eventAggregator, IOptionsController optionsController, 
            ISpecificationFactory specificationFactory)
        {
            this.model = model;
            this.eventAggregator = eventAggregator;
            this.optionsController = optionsController;
            this.specificationFactory = specificationFactory;

            UpdateMetadataOptions();
        }

        public void Search(string metadataType, string searchText)
        {
            var specification = specificationFactory.Create(metadataType, searchText);
            var filterTreeEvent = new FilterTreeEvent(specification);
            eventAggregator.Send(this, filterTreeEvent);
        }

        public void Handle(OptionsChanged @event)
        {
            UpdateMetadataOptions();
        }

        private void UpdateMetadataOptions()
        {
            var settings = optionsController.GetCurrentSettings();
            var metadata = new List<string>{ "Name" };
            metadata.AddRange(settings.TreeViewCategories);
            model.Metadata.Value = metadata;
        }
    }
}
