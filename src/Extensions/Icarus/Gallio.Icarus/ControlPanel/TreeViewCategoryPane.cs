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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TreeViewCategoryPane : PreferencePane
    {
        private readonly IOptionsController optionsController;
        private readonly IList<string> selectedCategories;
        private readonly IList<string> unselectedCategories;

        public TreeViewCategoryPane(IOptionsController optionsController)
        {
            InitializeComponent();

            this.optionsController = optionsController;
            
            selectedCategories = new BindingList<string>(optionsController.SelectedTreeViewCategories.Value);
            unselectedCategories = new BindingList<string>(optionsController.UnselectedTreeViewCategories.Value);

            selectedTreeViewCategories.DataSource = selectedCategories;
            unselectedTreeViewCategories.DataSource = unselectedCategories;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            object category = unselectedTreeViewCategories.SelectedItem;
            selectedCategories.Add((string)category);
            unselectedCategories.Remove((string)category);

            PendingSettingsChanges = true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            object category = selectedTreeViewCategories.SelectedItem;
            unselectedCategories.Add((string) category);
            selectedCategories.Remove((string) category);

            PendingSettingsChanges = true;
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, 
            IProgressMonitor progressMonitor)
        {
            optionsController.SelectedTreeViewCategories.Value = selectedCategories;
            optionsController.Save();

            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }
    }
}
