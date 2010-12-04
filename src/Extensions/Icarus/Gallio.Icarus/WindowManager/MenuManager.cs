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

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Gallio.Common;
using Gallio.UI.Menus;

namespace Gallio.Icarus.WindowManager
{
    public class MenuManager : IMenuManager
    {
        private readonly Dictionary<string, IMenuList> menus = new Dictionary<string, IMenuList>();
        private readonly IDictionary<string, IList<Func<MenuCommand>>> commandFactories = new Dictionary<string, IList<Func<MenuCommand>>>();
        private IList items;

        internal void SetToolstrip(IList toolStrip)
        {
            items = toolStrip;

            foreach (ToolStripMenuItem item in toolStrip)
            {
                var name = item.Text.Replace("&", "");
                menus.Add(name, new MenuList(item));
            }
            
            ExecuteCommandFactories();
        }

        private void ExecuteCommandFactories()
        {
            foreach (var menuId in commandFactories.Keys)
            {
                foreach (var commandFactory in commandFactories[menuId])
                {
                    AddMenuItem(menuId, commandFactory);
                }
            }
        }

        private IMenuList GetMenu(string menuId)
        {
            if (menus.ContainsKey(menuId) == false)
            {
                var item = new ToolStripMenuItem { Text = menuId };
                items.Add(item);

                var menuList = new MenuList(item);
                menus.Add(menuId, menuList);
            }

            return menus[menuId];
        }

        public void Add(string menuId, Func<MenuCommand> commandFactory)
        {
            if (items == null)
            {
                QueueMenuItem(menuId, commandFactory);
            }
            else
            {
                AddMenuItem(menuId, commandFactory);
            }
        }

        private void QueueMenuItem(string menuId, Func<MenuCommand> commandFactory)
        {
            if (commandFactories.ContainsKey(menuId) == false)
            {
                commandFactories.Add(menuId, new List<Func<MenuCommand>>());
            }
            commandFactories[menuId].Add(commandFactory);
        }

        private void AddMenuItem(string menuId, Func<MenuCommand> commandFactory)
        {
            var menuList = GetMenu(menuId);
            var menuCommand = commandFactory();
            menuList.Add(menuCommand);
        }
    }
}
