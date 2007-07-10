using System;
using System.Collections;

namespace TestFu.Operations
{
	public interface ITupleEnumeratorFactory
	{
		ITupleEnumerator Create(ITuple tuple);
	}
}
