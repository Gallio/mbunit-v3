namespace MbUnit.Framework
{
    /// <summary>
    /// A keyed pairwise assertion validates a relation between an expected and actual value
    /// that are related by a common key.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <param name="key">The common key associated with both values</param>
    /// <param name="expected">The expected value</param>
    /// <param name="actual">The actual value</param>
    public delegate void KeyedPairwiseAssertion<TKey, TValue>(TKey key, TValue expected, TValue actual);
}