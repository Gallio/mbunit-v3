using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Icarus.Plugins
{
    public class PluginHost : IMbUnitPluginHost
    {

        #region IMbUnitPluginHost Members
        
        public event ProjectLoadedDelegate ProjectLoaded;
        public event AssemblyAddedDelegate AssemblyAdded;
        public event AssemblyRemovedDelegate AssemblyRemoved;

        public void Feedback(string Feedback, IMbUnitPlugin Plugin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public void FireProjectLoaded(string projectName, string projectPath)
        {
            if (ProjectLoaded != null)
            {
                ProjectLoaded(projectName, projectPath);
            }
        }
    }
}
