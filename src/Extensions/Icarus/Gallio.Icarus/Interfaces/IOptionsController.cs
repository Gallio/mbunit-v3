using System.ComponentModel;
using System.Drawing;

namespace Gallio.Icarus.Interfaces
{
    public interface IOptionsController
    {
        bool AlwaysReloadAssemblies { get; set; }
        string TestProgressBarStyle { get; set; }
        bool ShowProgressDialogs { get; set; }
        bool RestorePreviousSettings { get; set; }
        string TestRunnerFactory { get; set; }
        string[] TestRunnerFactories { get; }
        BindingList<string> PluginDirectories { get; }
        Color PassedColor { get; set; }
        Color FailedColor { get; set; }
        Color InconclusiveColor { get; set; }
        Color SkippedColor { get; set; }
        
        void Save();
        
        void RemovePluginDirectory(string directory);
        void AddPluginDirectory(string directory);
    }
}
