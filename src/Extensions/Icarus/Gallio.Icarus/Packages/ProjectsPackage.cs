using System;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Views.Projects;
using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Packages
{
    internal class ProjectsPackage : IPackage
    {
        public static readonly string ProjectPropertiesWindowId = "Gallio.Icarus.ProjectProperties";

        public void Load(IServiceLocator serviceLocator)
        {
            // get the window manager
            var windowManager = serviceLocator.Resolve<IWindowManager>();

            var projectController = serviceLocator.Resolve<IProjectController>();

            // register an action to create the window on demand
            // (in case it is already open when dock state is restored)
            windowManager.Register(ProjectPropertiesWindowId, () =>
            {
                var projectPropertiesControl = new ProjectProperties(projectController);
                windowManager.Add(ProjectPropertiesWindowId, projectPropertiesControl, "Properties");
            });

            // find the "Project" menu item
            var menuItems = windowManager.Menu.Find("propertiesToolStripMenuItem", true);
            if (menuItems.Length != 1)
                throw new Exception("Could not find menu item");
            var menuItem = (ToolStripMenuItem)menuItems[0];
            menuItem.Click += (sender, e) => { windowManager.Show(ProjectPropertiesWindowId); };
        }

        public void Unload()
        {
        }
    }
}
