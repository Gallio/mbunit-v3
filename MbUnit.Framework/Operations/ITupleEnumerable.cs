using System;
using System.Collections;

namespace TestFu.Operations
{
	public interface ITupleEnumerable : IEnumerable
	{
		new ITupleEnumerator GetEnumerator();
	}
}
