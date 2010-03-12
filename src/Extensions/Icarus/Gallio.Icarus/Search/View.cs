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

using System.Windows.Forms;

namespace Gallio.Icarus.Search
{
    public partial class View : UserControl
    {
        private readonly IController controller;
        private readonly IModel model;

        public View(IController controller, IModel model)
        {
            this.controller = controller;
            this.model = model;

            InitializeComponent();

            SetMetadataOptions();
            model.Metadata.PropertyChanged += (s, e) => SetMetadataOptions();
        }

        private void SetMetadataOptions()
        {
            metadataComboBox.Items.Clear();
            foreach (var metadata in model.Metadata.Value)
            {
                metadataComboBox.Items.Add(metadata);
            }

            if (metadataComboBox.Items.Count > 0)
                metadataComboBox.SelectedIndex = 0;
        }

        private void searchTextBox_TextChanged(object sender, System.EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            controller.Search((string)metadataComboBox.SelectedItem, searchTextBox.Text);
        }

        private void metadataComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Search();
        }
    }
}
