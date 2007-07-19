namespace MbUnit.Framework.Kernel.Collections
{
    /// <summary>
    /// Provides a singleton empty array instance.
    /// </summary>
    /// <typeparam name="T">The type of array to provide</typeparam>
    public static class EmptyArray<T>
    {
        /// <summary>
        /// An empty array of type <typeparamref name="T"/>.
        /// </summary>
        public static readonly T[] Instance = new T[0];
    }
}