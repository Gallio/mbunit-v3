namespace Gallio.Icarus
{
    /// <summary>
    /// Shameless rip-off of Reflector add-in API.
    /// </summary>
    public interface IWindowManager
    {
        IStatusStrip StatusStrip { get; }
        IWindowCollection Windows { get; }
        IToolStripManager ToolStripManager { get; }
        IMenuManager MenuManager { get; }
    }
}
