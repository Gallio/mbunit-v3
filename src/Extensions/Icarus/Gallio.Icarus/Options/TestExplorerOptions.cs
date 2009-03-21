// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class TestExplorerOptions : OptionsPanel
    {
        private readonly IOptionsController optionsController;

        public TestExplorerOptions(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            selectedTreeViewCategories.DataSource = optionsController.SelectedTreeViewCategories;
            unselectedTreeViewCategories.DataSource = optionsController.UnselectedTreeViewCategories;

            alwaysReloadAssemblies.DataBindings.Add("Checked", optionsController, "AlwaysReloadAssemblies", false, 
                DataSourceUpdateMode.OnPropertyChanged);
            
            runTestsAfterReload.DataBindings.Add("Enabled", optionsController, "AlwaysReloadAssemblies", false, 
                DataSourceUpdateMode.OnPropertyChanged);
            runTestsAfterReload.DataBindings.Add("Checked", optionsController, "RunTestsAfterReload", false, 
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            object category = unselectedTreeViewCategories.SelectedItem;
            optionsController.SelectedTreeViewCategories.Add((string) category);
            optionsController.UnselectedTreeViewCategories.Remove((string) category);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            object category = selectedTreeViewCategories.SelectedItem;
            optionsController.UnselectedTreeViewCategories.Add((string) category);
            optionsController.SelectedTreeViewCategories.Remove((string) category);
        }
    }
}
