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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Gallio.UI.Properties;

namespace Gallio.UI.ControlPanel.Plugins
{
    internal partial class PluginControlPanelTab : ControlPanelTab
    {
        public PluginControlPanelTab()
        {
            InitializeComponent();
        }

        public void AddPlugin(string id, string name, Version version,
            Icon icon, string description, string disabledReason)
        {
            if (icon == null)
                icon = Resources.DefaultPluginIcon;

            Image smallIcon = new Icon(icon, new Size(16, 16)).ToBitmap();
            Image bigIcon = new Icon(icon, new Size(32, 32)).ToBitmap();

            var pluginInfo = new PluginInfo()
            {
                Id = id,
                Name = name,
                Version = version != null ? version.ToString() : "",
                SmallIcon = smallIcon,
                BigIcon = bigIcon,
                Description = description ?? name,
                DisabledReason = disabledReason
            };

            int rowIndex = pluginGrid.Rows.Add(pluginInfo.SmallIcon, pluginInfo.Name, pluginInfo.Version);
            DataGridViewRow row = pluginGrid.Rows[rowIndex];
            row.Tag = pluginInfo;

            if (disabledReason != null)
                row.DefaultCellStyle.ForeColor = Color.LightGray;
        }

        private void pluginGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (pluginGrid.SelectedRows.Count != 0)
            {
                var row = pluginGrid.SelectedRows[0];
                var pluginInfo = (PluginInfo)row.Tag;

                pluginIconPictureBox.Image = pluginInfo.BigIcon;

                StringBuilder rtf = new StringBuilder();
                rtf.Append(@"{\rtf1 ");
                rtf.Append(RtfEscape(pluginInfo.Description));

                if (pluginInfo.DisabledReason != null)
                {
                    rtf.Append(@"\par\b Disabled:\b0  ");
                    rtf.Append(RtfEscape(pluginInfo.DisabledReason));
                }

                rtf.Append(@"}");

                pluginDescriptionRichTextBox.Rtf = rtf.ToString();
            }
            else
            {
                pluginIconPictureBox.Image = null;
                pluginDescriptionRichTextBox.Clear();
            }
        }

        private static string RtfEscape(string text)
        {
            return text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
        }

        private sealed class PluginInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
            public Image SmallIcon { get; set; }
            public Image BigIcon { get; set; }
            public string Description { get; set; }
            public string DisabledReason { get; set; }
        }

        private void PluginControlPanelTab_Load(object sender, EventArgs e)
        {
            pluginGrid.Sort(pluginNameColumn, ListSortDirection.Ascending);
        }
    }
}
