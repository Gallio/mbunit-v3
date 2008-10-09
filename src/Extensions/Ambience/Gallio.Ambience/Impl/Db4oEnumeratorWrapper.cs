using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Ext;
using Gallio.Ambience.Properties;

namespace Gallio.Ambience.Impl
{
    internal sealed class Db4oEnumeratorWrapper<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> inner;

        public Db4oEnumeratorWrapper(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        public void Dispose()
        {
            try
            {
                inner.Dispose();
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public bool MoveNext()
        {
            try
            {
                return inner.MoveNext();
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public void Reset()
        {
            try
            {
                inner.Reset();
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
            }
        }

        public T Current
        {
            get
            {
                try
                {
                    return inner.Current;
                }
                catch (Db4oException ex)
                {
                    throw new AmbienceException(Resources.Db4oAmbientDataContainer_QueryResultException, ex);
                }
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
