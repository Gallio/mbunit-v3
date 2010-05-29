using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.UI.Common.Synchronization;

namespace Gallio.UI.DataBinding
{
    ///<summary>
    /// List that raises an event when the contents are modified.
    ///</summary>
    /// <remarks>Wraps a List&lt;T&gt;.</remarks>
    ///<typeparam name="T">The type of elements in the list.</typeparam>
    public class ObservableList<T> : IList<T>, INotifyPropertyChanged
    {
        private readonly List<T> innerList = new List<T>();

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            innerList.Add(item);
            OnPropertyChanged("Add");
        }

        /// <inheritdoc />
        public void Clear()
        {
            innerList.Clear();
            OnPropertyChanged("Clear");
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return innerList.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            var removed = innerList.Remove(item);
            OnPropertyChanged("Remove");
            return removed;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return innerList.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return innerList.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            innerList.Insert(index, item);
            OnPropertyChanged("Insert");
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
            OnPropertyChanged("Remove");
        }

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
                OnPropertyChanged("Update");
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            var eventArgs = new PropertyChangedEventArgs(propertyName);

            if (SynchronizationContext.Current != null)
            {
                SynchronizationContext.Post(delegate
                {
                    PropertyChanged(this, eventArgs);
                }, this);
            }
            else
            {
                PropertyChanged(this, eventArgs);
            }
        }

        /// <summary>
        /// Copies the elements of the list to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the list.</returns>
        public T[] ToArray()
        {
            return innerList.ToArray();
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection)
        {
            innerList.AddRange(collection);
        }
    }
}
