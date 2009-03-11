using System.Windows.Forms;

namespace Gallio.Icarus
{
    /// <summary>
    /// Exposes the status strip at the bottom of Icarus.
    /// </summary>
    public interface IStatusStrip
    {
        ToolStripItemCollection Items { get; }
    }
}
