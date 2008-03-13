// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.IO;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;
using Gallio.Runner.Projects;

namespace Gallio.Icarus
{
    public partial class FiltersWindow : DockWindow
    {
        private IProjectAdapterView projectAdapterView;

        public IList<FilterInfo> Filters
        {
            set
            {
                // populate list box
                filtersListBox.Items.Clear();
                foreach (FilterInfo filterInfo in value)
                    filtersListBox.Items.Add(filterInfo);
            }
        }

        public FiltersWindow(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();
        }

        private void filtersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeFilterButton.Enabled = applyFilterButton.Enabled = (filtersListBox.SelectedItems.Count > 0);
        }

        private void removeFilterButton_Click(object sender, EventArgs e)
        {
            projectAdapterView.DeleteFilter((FilterInfo)filtersListBox.SelectedItem);
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
                string title = "Duplicate test filter";
                string message = "A test filter with that name already exists. Please choose another.";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                projectAdapterView.SaveFilter(filterNameTextBox.Text);
                filterNameTextBox.Clear();
            }
        }

        private void applyFilterButton_Click(object sender, EventArgs e)
        {
            projectAdapterView.ApplyFilter(((FilterInfo)filtersListBox.SelectedItem).Filter);
        }
    }
}
