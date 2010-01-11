namespace Gallio.Icarus.Events
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


