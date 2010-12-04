using System;

namespace Gallio.Copy.Controllers
{
    public class PluginFolderUpdatedEvent : EventArgs
    {
        public string Folder { get; private set; }

        public PluginFolderUpdatedEvent(string folder)
        {
            Folder = folder;
        }
    }
}