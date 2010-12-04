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
using System.Windows.Forms;
using Gallio.Runtime.Extensibility;
using Gallio.UI.Tree.Nodes;

namespace Gallio.Copy.Model
{
    public class PluginNode : ThreeStateNode
    {        
        private readonly List<string> dependencies = new List<string>();

        public IPluginDescriptor Plugin { get; private set; }

        public PluginNode(IPluginDescriptor plugin) : base(plugin.PluginId)
        {
            CheckState = CheckState.Checked;
            Plugin = plugin;
            foreach (var dependency in plugin.PluginDependencies)
            {
                dependencies.Add(dependency.PluginId);
            }
        }

        public override CheckState CheckState
        {
            get
            {
                return base.CheckState;
            }
            set
            {
                base.CheckState = value;
                if (value == CheckState.Checked)
                    UpdateDependencies();
            }
        }

        private void UpdateDependencies()
        {
            if (Parent == null)
                return;

            foreach (PluginNode pluginNode in Parent.Nodes)
            {
                if (dependencies.Contains(pluginNode.Text))
                    pluginNode.CheckState = CheckState.Checked;
            }
        }
    }
}
