using System;
using System.Collections.Generic;
using Gallio.Common.IO;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Utilities;
using System.Text;

namespace Gallio.Icarus.Helpers
{
    internal class MenuListHelper
    {
        public static List<ToolStripMenuItem> GetRecentProjectsMenuList(MRUList list, Action<string> action, IFileSystem fileSystem)
        {
            var menuItems = new List<ToolStripMenuItem>();
            
            foreach (var item in list.Items)
            {
                // copy string for click delegate
                string name = item;

                // don't add any items that don't exist on disk
                if (!fileSystem.FileExists(item))
                    continue;

                var menuItem = new ToolStripMenuItem();

                // shorten path for text by inserting ellipsis (...)
                string text = item;
                if (text.Length > 60)
                    text = TruncatePath(item, 60);
                menuItem.Text = text;

                menuItem.Click += delegate { action(name); };
                menuItems.Add(menuItem);
            }

            return menuItems;
        }

        private static string TruncatePath(string path, int length)
        {
            StringBuilder sb = new StringBuilder();
            NativeMethods.PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }
    }
}
