using System;
using System.Collections;

namespace TestFu.Operations
{
    internal class UniformTWizeProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        private int tupleSize;
        public UniformTWizeProductDomainTupleEnumerable(IDomainCollection domains, int tupleSize)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");
            if (tupleSize <= 0)
                throw new ArgumentOutOfRangeException("tupleSize is negative or zero");

            this.domains = domains;
            this.tupleSize = tupleSize;

            int count = -1;
            for (int i = 0; i < domains.Count; ++i)
            {
                if (i == 0)
                    count = domains[i].Count;
                else
                {
                    if (count != domains[i].Count)
                        throw new ArgumentException("Domains have not uniform size");
                }
            }
        }

        public IDomainCollection Domains
        {
            get
            {
                return this.domains;
            }
        }

        public ITupleEnumerator GetEnumerator()
        {
            return new UniformTWizeProductDomainTupleEnumerator(this.Domains, this.tupleSize);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal sealed class UniformTWizeProductDomainTupleEnumerator :
            DomainTupleEnumeratorBase
        {
            private int tupleSize;
            public UniformTWizeProductDomainTupleEnumerator(IDomainCollection domains, int tupleSize)
                :base(domains)
            {
                this.tupleSize = tupleSize;
                this.CreateColoring();
            }

            public override void Reset()
            {
                throw new NotImplementedException();
            }
            public override bool MoveNext()
            {
                throw new NotImplementedException();
            }
            public override ITuple Current
            {
                get { throw new NotImplementedException(); }
            }

            private void CreateColoring()
            {
            }
        }
    }
}
