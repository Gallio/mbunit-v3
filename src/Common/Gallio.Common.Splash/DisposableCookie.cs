using System;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Invokes a delegate when disposed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is useful for creating APIs that leverage the C# "using" syntax for scoping.
    /// </para>
    /// <example>
    /// <code>
    /// using (document.BeginStyle(Style.Default)) // returns a DisposableCookie
    /// {
    ///     document.AppendText("Hello world");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public struct DisposableCookie : IDisposable
    {
        private readonly Action action;

        /// <summary>
        /// Specifies the action to perform when <see cref="Dispose"/> is called.
        /// </summary>
        public delegate void Action();

        /// <summary>
        /// Creates a disposable cookie.
        /// </summary>
        /// <param name="action">The action to perform when <see cref="Dispose"/> is called.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public DisposableCookie(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.action = action;
        }

        /// <summary>
        /// Invokes the dispose action.
        /// </summary>
        public void Dispose()
        {
            action();
        }
    }
}
