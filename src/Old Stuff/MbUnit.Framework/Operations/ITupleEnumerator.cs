using System;
using System.Collections;

namespace TestFu.Operations
{
	public interface ITupleEnumerator : IEnumerator
	{
		new ITuple Current {get;}
	}
}
