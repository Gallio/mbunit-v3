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
        {
            windowManager.Remove(windowId);
        }
    }
}
