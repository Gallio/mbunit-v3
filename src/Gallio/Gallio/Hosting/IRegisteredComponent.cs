namespace Gallio.Hosting
{
    /// <summary>
    /// A registered component is a component that has a name and description.
    /// </summary>
    public interface IRegisteredComponent
    {
        /// <summary>
        /// Gets the unique name of the component.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the human-readable description of the component.
        /// </summary>
        string Description { get; }
    }
}
