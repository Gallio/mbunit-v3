using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// A pairwise assertion validates a relation between an expected and actual value.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <param name="expected">The expected value</param>
    /// <param name="actual">The actual value</param>
    public delegate void PairwiseAssertion<T>(T expected, T actual);
}
