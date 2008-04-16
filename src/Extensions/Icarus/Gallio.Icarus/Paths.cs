using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gallio.Icarus
{
    public static class Paths
    {
        public static string IcarusAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Gallio\Icarus");
        public static string DefaultProject = Path.Combine(IcarusAppDataFolder, @"Icarus.gallio");
        public static string SettingsFile = Path.Combine(IcarusAppDataFolder, @"Icarus.settings");
        public static string DockConfigFile = Path.Combine(IcarusAppDataFolder, @"DockPanel.config");
    }
}
