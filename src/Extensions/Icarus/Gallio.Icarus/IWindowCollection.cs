using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Icarus
{
    /// <summary>
    /// Shameless rip-off of Reflector add-in API.
    /// </summary>
    public interface IWindowCollection
    {
        IWindow Add(string identifier, Control content, string caption);
        IWindow Add(string identifier, Control content, string caption, Icon icon);
        void Remove(string identifier);
        IWindow this[string identifier] { get; }
    }
}
