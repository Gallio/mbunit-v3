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
using System.Windows.Forms;
using Gallio.Icarus.Views.PluginBrowser;
using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Packages
{
    internal class PluginBrowserPackage : IPackage
    {
        private IWindowManager windowManager;
        private readonly string windowId = "Gallio.Icarus.PluginBrowser";

        public void Load(IServiceLocator serviceLocator)
        {
            // get the window manager
            windowManager = serviceLocator.Resolve<IWindowManager>();

            // register an action to create the window on demand
            windowManager.Register(windowId, () =>
            {
                var pluginBrowserControl = new PluginBrowser();
                windowManager.Add(windowId, pluginBrowserControl, "Plugin Browser");
            });

            // find the "Tools" menu item
            var menuItems = windowManager.Menu.Find("toolsToolStripMenuItem", false);
            if (menuItems.Length != 1)
                throw new Exception("Could not find menu item");
            var menuItem = (ToolStripMenuItem)menuItems[0];
            
            // add a new "Plugin Browser" menu item
            var pluginBrowserMenuItem = new ToolStripMenuItem("Plugin Browser");
            pluginBrowserMenuItem.Click += (sender, e) => 
            {
                windowManager.Show(windowId);
            };
            menuItem.DropDownItems.Add(pluginBrowserMenuItem);
        }

        public void Unload()
        { }
    }
}
