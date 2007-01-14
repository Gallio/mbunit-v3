using System;
using System.Collections;

namespace TestFu.Operations
{
	internal abstract class DomainTupleEnumeratorBase : ITupleEnumerator
	{
		private IDomainCollection domains;
        public DomainTupleEnumeratorBase(IDomainCollection domains)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");
            this.domains = domains;
            foreach(IDomain domain in domains)
            {
                if (domain.Count == 0)
                    throw new ArgumentException("A domain is empty");
            }
        }
		
		public IDomainCollection Domains
		{
			get
			{
				return this.domains;
			}
		}
		
		public abstract void Reset();
		public abstract bool MoveNext();
		public abstract ITuple Current {get;}
		
		Object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}
	}
}
