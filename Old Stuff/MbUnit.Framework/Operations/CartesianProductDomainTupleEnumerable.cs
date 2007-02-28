using System;
using System.Collections;

namespace TestFu.Operations
{
    internal class CartesianProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        public CartesianProductDomainTupleEnumerable(IDomainCollection domains)
        {
            this.domains = domains;
        }
        public ITupleEnumerator GetEnumerator()
        {
            return new CartesianProductDomainTupleEnumerator(this.domains);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
