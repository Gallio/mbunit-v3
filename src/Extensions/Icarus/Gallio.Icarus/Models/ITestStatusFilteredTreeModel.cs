using Aga.Controls.Tree;

namespace Gallio.Icarus.Models
{
    /// <summary>
    /// Marker interface to allow the IoC container to wire up
    /// the model decorators in the correct order.
    /// </summary>
    public interface ITestStatusFilteredTreeModel : ITreeModel
    {
    }
}