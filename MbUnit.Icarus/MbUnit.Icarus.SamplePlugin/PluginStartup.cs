using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using MbUnit.Icarus.Plugins;

namespace MbUnit.Icarus.SamplePlugin
{
    public class PluginStartup : IMbUnitPlugin
    {
        private string name = "Sample Plugin";
        private string description = "An example of the MbUnit plugin framework.";
        private string author = "David Parkinson";

        private Version version = Assembly.GetExecutingAssembly().GetName().Version;

        private IMbUnitPluginHost host;

        #region IMbUnitPlugin Members

        public IMbUnitPluginHost Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public string Author
        {
            get { return this.author; }
        }

        public string Version
        {
            get { return this.version.ToString(); }
        }

        public UserControl MainInterface
        {
            get { return null; }
        }

        public OptionsTreeNode OptionsMenu
        {
            get
            {
                OptionsTreeNode menu = new OptionsTreeNode("Plugin Menu Item");

                OptionsTreeNode node1 = new OptionsTreeNode("Sub Menu Item", new OptionsTreeNodeSelectedHandler(DisplayOptions));
                menu.Nodes.Add(node1);

                OptionsTreeNode node2 = new OptionsTreeNode("Another Item", new OptionsTreeNodeSelectedHandler(DisplayOtherOptions));
                menu.Nodes.Add(node2);

                return menu;
            }
        }

        private UserControl DisplayOptions()
        {
            return new PluginOptions();
        }

        private UserControl DisplayOtherOptions()
        {
            return new AnotherOptionsPanel();
        }

        public void Initialize()
        {
            if (this.host != null)
            {
                // Register the events we want to listen to.
                this.host.ProjectLoaded += new ProjectLoadedDelegate(Host_ProjectLoaded);
            }
        }

        public void Dispose()
        {

        }

        #endregion

        void Host_ProjectLoaded(string projectName, string projectPath)
        {
            MessageBox.Show(projectName + " :: " + projectPath);
        }
    }
}
