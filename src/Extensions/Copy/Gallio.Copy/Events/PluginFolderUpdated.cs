using Gallio.UI.Events;

namespace Gallio.Copy.Events
{
    public class PluginFolderUpdated : Event
    {
        public string Folder { get; private set; }

        public PluginFolderUpdated(string folder)
        {
            Folder = folder;
        }
    }
}