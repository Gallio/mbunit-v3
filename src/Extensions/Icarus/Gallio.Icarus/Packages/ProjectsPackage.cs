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
