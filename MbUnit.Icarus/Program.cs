using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MbUnit.Icarus.Plugins;

namespace MbUnit.Icarus
{
    static class Program
    {
        public static PluginServices Plugins = new PluginServices();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Call the find plugins routine, to search in our Plugins Folder
            Plugins.FindPlugins(Application.StartupPath + @"\Plugins");

            Application.Run(new Main());
        }
    }
}