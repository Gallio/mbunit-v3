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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Views.Projects;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.Packages
{
    public class ProjectsPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IProjectController projectController;
        public static readonly string projectPropertiesWindowId = "Gallio.Icarus.ProjectProperties";

        public ProjectsPackage(IWindowManager windowManager, 
            IProjectController projectController)
        {
            this.windowManager = windowManager;
            this.projectController = projectController;
        }

        public void Load()
        {
            RegisterWindow();
            AddMenuItem();
        }

        private void AddMenuItem()
        {
            var menu = windowManager.MenuManager.GetMenu("Project");

            var menuCommand = new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(projectPropertiesWindowId)),
                Text = Resources.ProjectsPackage_AddMenuItem_Properties,
                Image = Resources.PropertiesImage
            };

            menu.Add(menuCommand);
        }

        private void RegisterWindow()
        {
            windowManager.Register(projectPropertiesWindowId, () =>
            {
                var projectPropertiesControl = new ProjectProperties(projectController);
                windowManager.Add(projectPropertiesWindowId, projectPropertiesControl, 
                    Resources.ProjectsPackage_AddMenuItem_Properties);
            });
        }

        public void Dispose() { }
    }
}
