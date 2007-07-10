using System;
using System.Collections;

namespace TestFu.Operations
{
    public class DomainCollection : CollectionBase, IDomainCollection
    {
        public DomainCollection(IDomain[] domains)
        {
            foreach (IDomain domain in domains)
                this.Add(domain);
        }
        public DomainCollection()
        {}

        public IDomain this[int index]
        {
            get
            {
                return (IDomain)this.List[index];
            }
        }

        public void Add(IDomain domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");
            this.List.Add(domain);
        }

        public new IDomainEnumerator GetEnumerator()
        {
            return new DomainEnumerator(this.List.GetEnumerator());
        }

        internal class DomainEnumerator : IDomainEnumerator
        {
            private IEnumerator en;
            public DomainEnumerator(IEnumerator en)
            {
                this.en = en;
            }

            public IDomain Current
            {
                get
                {
                    return (IDomain)this.en.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Reset()
            {
                this.en.Reset();
            }

            public bool MoveNext()
            {
                return this.en.MoveNext();
            }
        }
    }
}
