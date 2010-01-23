using Gallio.UI.Controls;
using Gallio.UI.Menus;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;

namespace Gallio.Icarus.WindowManager
{
    public class MenuList : IMenuList
    {
        private readonly ToolStripMenuItem item;

        public MenuList(ToolStripMenuItem item)
        {
            this.item = item;
        }

        public void Add(MenuCommand menuCommand)
        {
            var menuItem = new CommandToolStripMenuItem(menuCommand);
            item.DropDownItems.Add(menuItem);
        }
    }
}