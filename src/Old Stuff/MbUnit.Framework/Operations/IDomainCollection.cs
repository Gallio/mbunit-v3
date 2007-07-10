using System;
using System.Collections;

namespace TestFu.Operations
{
	public interface IDomainCollection : ICollection
	{
		new IDomainEnumerator GetEnumerator();

        IDomain this[int i] { get;}
        void Add(IDomain domain);
    }
}
