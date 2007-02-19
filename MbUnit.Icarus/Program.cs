using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MbUnit.Icarus.Plugins;

namespace MbUnit.Icarus
{
    static class Program
    {
        private static PluginServices plugins = new PluginServices();
        private static PluginHost host = new PluginHost();

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

        public static PluginHost Host
        {
            get { return host; }
        }

        public static PluginServices Plugins
        {
            get { return plugins; }
        }
    }
}