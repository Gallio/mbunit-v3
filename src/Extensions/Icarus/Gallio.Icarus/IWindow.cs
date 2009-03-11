using System.Windows.Forms;

namespace Gallio.Icarus
{
    /// <summary>
    /// Shameless rip-off of Reflector add-in API.
    /// </summary>
    public interface IWindow
    {
        string Caption { get; }
        Control Content { get; }
        bool Visible { get; set; }
    }
}
