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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Views.PluginBrowser;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.Packages
{
    public class PluginBrowserPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private const string WindowId = "Gallio.Icarus.PluginBrowser";

        public PluginBrowserPackage(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void Load()
        {
            RegisterWindow();

            AddMenuItem();
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("Tools", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = Resources.PluginBrowserPackage_AddMenuItem_Plugin_Browser
            });
        }

        private void RegisterWindow()
        {
            // register an action to create the window on demand
            windowManager.Register(WindowId, () =>
            {
                var pluginBrowserControl = new PluginBrowser();
                var caption = Resources.PluginBrowserPackage_AddMenuItem_Plugin_Browser;
                windowManager.Add(WindowId, pluginBrowserControl, caption);
            });
        }

        public void Dispose() { }
    }
}
