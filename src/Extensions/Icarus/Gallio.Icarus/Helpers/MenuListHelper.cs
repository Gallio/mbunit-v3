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
