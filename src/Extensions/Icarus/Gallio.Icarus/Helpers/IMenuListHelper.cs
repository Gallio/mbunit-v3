using System;
using Gallio.Icarus.Controls;

namespace Gallio.Icarus.Helpers
{
    interface IMenuListHelper
    {
        ToolStripMenuItem[] GetRecentProjectsMenuList(Action<string> action);
    }
}
