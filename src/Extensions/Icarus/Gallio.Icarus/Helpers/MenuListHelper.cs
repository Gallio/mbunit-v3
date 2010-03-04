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
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.Controls;

namespace Gallio.Icarus.Helpers
{
    internal class MenuListHelper : IMenuListHelper
    {
        private readonly IOptionsController optionsController;
        private readonly IFileSystem fileSystem;

        public MenuListHelper(IOptionsController optionsController, IFileSystem fileSystem)
        {
            this.optionsController = optionsController;
            this.fileSystem = fileSystem;
        }

        public ToolStripMenuItem[] GetRecentProjectsMenuList(Action<string> action)
        {
            var menuItems = new List<ToolStripMenuItem>();
            
            foreach (var recentProject in optionsController.RecentProjects.Items)
            {
                // copy string for click delegate
                string name = recentProject;

                // don't add any items that don't exist on disk
                if (!fileSystem.FileExists(recentProject))
                    continue;

                var menuItem = new TruncatedToolStripMenuItem(recentProject, 60);
                menuItem.Click += delegate { action(name); };
                menuItems.Add(menuItem);
            }

            return menuItems.ToArray();
        }
    }
}
