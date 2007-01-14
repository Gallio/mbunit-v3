using System;
using System.Collections;

namespace TestFu.Operations
{
	public interface IDomainEnumerator : IEnumerator
	{
		new IDomain Current {get;}
	}
}
