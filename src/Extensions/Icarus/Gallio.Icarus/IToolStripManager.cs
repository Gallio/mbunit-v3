using System.Windows.Forms;

namespace Gallio.Icarus
{
    public interface IToolStripManager
    {
        ToolStripPanel Top { get; }
        ToolStripPanel Bottom { get; }
        ToolStripPanel Left { get; }
        ToolStripPanel Right { get; }
    }
}
