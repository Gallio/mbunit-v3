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