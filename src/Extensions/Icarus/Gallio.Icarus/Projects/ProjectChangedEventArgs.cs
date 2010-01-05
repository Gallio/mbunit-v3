using System;

namespace Gallio.Icarus.Projects
{
    public class ProjectChangedEventArgs : EventArgs
    {
        public string ProjectLocation { get; private set; }

        public ProjectChangedEventArgs(string projectLocation)
        {
            ProjectLocation = projectLocation;
        }
    }
}