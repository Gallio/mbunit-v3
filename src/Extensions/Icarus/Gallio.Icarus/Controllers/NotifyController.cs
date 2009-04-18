using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.Controllers
{
    public class NotifyController : INotifyController
    {
        public ISynchronizationContext SynchronizationContext { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext == null || PropertyChanged == null)
                return;

            SynchronizationContext.Send(delegate
            {
                PropertyChanged(this, e);
            }, this);
        }
    }
}
