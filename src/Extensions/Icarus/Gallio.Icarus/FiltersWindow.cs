// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Runner.Projects;

namespace Gallio.Icarus
{
    public partial class FiltersWindow : DockWindow
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;

        public FiltersWindow(IProjectController projectController, ITestController testController)
        {
            this.projectController = projectController;
            this.testController = testController;

            InitializeComponent();

            filtersListBox.DataSource = projectController.TestFilters;
            filtersListBox.DisplayMember = "FilterName";
        }

        private void filtersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeFilterButton.Enabled = applyFilterButton.Enabled = (filtersListBox.SelectedItems.Count > 0);
        }

        private void removeFilterButton_Click(object sender, EventArgs e)
        {
            projectController.DeleteFilter((FilterInfo)filtersListBox.SelectedItem);
        }

        private void filterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            saveFilterButton.Enabled = filterNameTextBox.Text.Length > 0;
        }

        private void saveFilterButton_Click(object sender, EventArgs e)
        {
            if (filtersListBox.Items.Contains(filterNameTextBox.Text))
            {
                // TODO: Localisation 
                const string title = "Duplicate test filter";
                const string message = "A test filter with that name already exists. Please choose another.";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                projectController.SaveFilter(filterNameTextBox.Text, testController.GetCurrentFilter());
                filterNameTextBox.Clear();
            }
        }

        private void applyFilterButton_Click(object sender, EventArgs e)
        {
            FilterInfo filterInfo = (FilterInfo)filtersListBox.SelectedItem;
            testController.ApplyFilter(filterInfo.Filter);
        }
    }
}
