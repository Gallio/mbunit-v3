using System;
using System.Windows.Forms;

namespace MbUnit.Icarus.Plugins
{
    public delegate void ProjectLoadedDelegate(string projectName, string projectPath);
    public delegate void AssemblyAddedDelegate(string assemblyName);
    public delegate void AssemblyRemovedDelegate(string assemblyName);

    public interface IMbUnitPlugin
    {
        IMbUnitPluginHost Host { get; set;}

        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        UserControl MainInterface { get; }

        OptionsTreeNode OptionsMenu { get; }

        void Initialize();
        void Dispose();

    }

    public interface IMbUnitPluginHost
    {
        event ProjectLoadedDelegate ProjectLoaded;
        event AssemblyAddedDelegate AssemblyAdded;
        event AssemblyRemovedDelegate AssemblyRemoved;

        void Feedback(string Feedback, IMbUnitPlugin Plugin);
    }
}
