using Gallio.Icarus.Utilities;
using System.ComponentModel;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface INotifyController : INotifyPropertyChanged
    {
        ISynchronizationContext SynchronizationContext { get; set; }
    }
}
