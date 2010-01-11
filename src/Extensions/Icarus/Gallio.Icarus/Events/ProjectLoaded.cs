namespace Gallio.Icarus.Events
{
    public class ProjectLoaded : Event
    {
        public string ProjectLocation { get; private set; }

        public ProjectLoaded(string projectLocation)
        {
            ProjectLocation = projectLocation;
        }
    }
}