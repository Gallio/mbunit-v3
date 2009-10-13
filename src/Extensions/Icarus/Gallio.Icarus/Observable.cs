using System.ComponentModel;
using Gallio.UI.Common.Synchronization;

namespace Gallio.Icarus
{
    // Thanks Oren!
    // http://ayende.com/Blog/archive/2009/08/08/an-easier-way-to-manage-inotifypropertychanged.aspx
    public class Observable<T> : INotifyPropertyChanged
    {
        private T value;

        public Observable()
        { }

        public Observable(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
             
                if (PropertyChanged == null)
                    return;

                SynchronizationContext.Send(delegate
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }, this);
            }
        }

        public static implicit operator T(Observable<T> val)
        {
            return val.value;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
