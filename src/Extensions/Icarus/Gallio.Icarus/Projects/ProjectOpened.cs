using Gallio.Icarus.Events;

namespace Gallio.Icarus.Projects
{
    public class ProjectOpened : Event
    {
        public string ProjectLocation { get; private set; }

        public ProjectOpened(string projectLocation)
        {
            ProjectLocation = projectLocation;
        }
    }
}
