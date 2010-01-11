namespace Gallio.Icarus.Events
{
    public class ProjectSaved : Event
    {
        public string ProjectLocation { get; private set; }

        public ProjectSaved(string projectLocation)
        {
            ProjectLocation = projectLocation;
        }
    }
}