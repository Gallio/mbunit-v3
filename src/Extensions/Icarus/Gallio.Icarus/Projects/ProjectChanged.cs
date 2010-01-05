using Gallio.Icarus.Events;

namespace Gallio.Icarus.Projects
{
    public class ProjectChanged : Event
    {
        public string ProjectLocation { get; private set; }

        public ProjectChanged(string projectLocation)
        {
            ProjectLocation = projectLocation;
        }
    }
}