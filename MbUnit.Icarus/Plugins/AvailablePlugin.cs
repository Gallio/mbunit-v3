using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Icarus.Plugins
{
    /// <summary>
    /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class AvailablePlugin
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private IMbUnitPlugin myInstance = null;
        private string myAssemblyPath = "";

        public IMbUnitPlugin Instance
        {
            get { return myInstance; }
            set { myInstance = value; }
        }

        public string AssemblyPath
        {
            get { return myAssemblyPath; }
            set { myAssemblyPath = value; }
        }
    }
}
