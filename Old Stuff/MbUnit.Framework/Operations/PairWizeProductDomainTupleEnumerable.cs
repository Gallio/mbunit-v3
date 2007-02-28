using System;
using System.Collections;

namespace TestFu.Operations
{
    internal class PairWizeProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        public PairWizeProductDomainTupleEnumerable(IDomainCollection domains)
        {
            if (domains==null)
                throw new ArgumentNullException("domains");
            this.domains = domains;
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
            return new PairWizeProductDomainTupleEnumerator(this.Domains);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal sealed class PairWizeProductDomainTupleEnumerator : DomainTupleEnumeratorBase
        {
            public PairWizeProductDomainTupleEnumerator(IDomainCollection domains)
		    :base(domains)
            {
                this.Reset();
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
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
