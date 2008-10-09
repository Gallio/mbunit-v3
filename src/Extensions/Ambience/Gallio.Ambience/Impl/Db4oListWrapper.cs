using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Ext;
using Gallio.Ambience.Properties;
using IEnumerator=System.Collections.IEnumerator;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// This wrapper reinterprets Db4o exceptions as Ambience exceptions so that the client
    /// can catch them in a meaningful way.  (Db4o is internalized by Ambience so its exception
    /// types are not accessible to clients.)
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    internal sealed class Db4oListWrapper<T> : IAmbientDataSet<T>
    {
        private readonly IList<T> inner;

        public Db4oListWrapper(IList<T> inner)
        {
            this.inner = inner;
        }

        public T this[int index]
        {
            get
            {
                try
                {
                    return inner[index];
                }
                catch (Db4oException ex)
                {
                    throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
                }

            }
            set { throw new NotSupportedException(); }
        }

        public int Count
        {
            get
            {
                try
                {
                    return inner.Count;
                }
                catch (Db4oException ex)
                {
                    throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
                }
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Contains(T item)
        {
            try
            {
                return inner.Contains(item);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                inner.CopyTo(array, arrayIndex);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public int IndexOf(T item)
        {
            try
            {
                return inner.IndexOf(item);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                return new Db4oEnumeratorWrapper<T>(inner.GetEnumerator());
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}
