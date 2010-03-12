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
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;

namespace Gallio.Icarus.WindowManager
{
    public class MenuManager : IMenuManager
    {
        private readonly ToolStripItemCollection items;
        private readonly Dictionary<string, IMenuList> menus = new Dictionary<string, IMenuList>();

        public MenuManager(ToolStripItemCollection items)
        {
            this.items = items;

            foreach (ToolStripMenuItem item in items)
            {
                var name = item.Text.Replace("&", "");
                menus.Add(name, new MenuList(item));
            }
        }

        public IMenuList GetMenu(string menuId)
        {
            if (menus.ContainsKey(menuId))
            {
                return menus[menuId];
            }

            var item = new ToolStripMenuItem { Text = menuId };
            items.Add(item);

            var menuList = new MenuList(item);
            menus.Add(menuId, menuList);
            return menuList;
        }
    }
}
