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

        public string[] OptionsMenu
        {
            get
            {
                return new string[] { "Plugin Menu Item" };
            }
        }

        public void Initialize()
        {
            MessageBox.Show("Sample Plugin Has Been Loaded");
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}
