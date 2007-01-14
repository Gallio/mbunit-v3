using System;
using System.Collections;

namespace TestFu.Operations
{
    internal class GreedyTupleEnumerable : ITupleEnumerable
    {
        private ITupleEnumerable enumerable;
        public GreedyTupleEnumerable(ITupleEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            this.enumerable = enumerable;
        }

        public ITupleEnumerator GetEnumerator()
        {
            return new GreedyTupleEnumerator(this.enumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal class GreedyTupleEnumerator : ITupleEnumerator
        {
            private Hashtable tuples = new Hashtable();
            private ITupleEnumerator enumerator;

            public GreedyTupleEnumerator(ITupleEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public void Reset()
            {
                this.tuples.Clear();
                this.enumerator.Reset();
            }

            public bool MoveNext()
            {
                while (this.enumerator.MoveNext())
                {
                    if (this.tuples.Contains(this.Current))
                        continue;
                    this.tuples.Add(this.Current, null);
                    return true;
                }
                return false;
            }

            public ITuple Current
            {
                get
                {
                    return this.enumerator.Current;
                }
            }

            Object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
        }
    }
}
