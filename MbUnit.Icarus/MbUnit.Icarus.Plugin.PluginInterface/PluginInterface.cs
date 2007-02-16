using System;
using System.Windows.Forms;

namespace MbUnit.Icarus.Plugins
{
    public interface IMbUnitPlugin
    {
        IMbUnitPluginHost Host { get; set;}

        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        UserControl MainInterface { get; }

        string[] OptionsMenu { get; }

        void Initialize();
        void Dispose();

    }

    public interface IMbUnitPluginHost
    {
        void Feedback(string Feedback, IMbUnitPlugin Plugin);
    }
}
