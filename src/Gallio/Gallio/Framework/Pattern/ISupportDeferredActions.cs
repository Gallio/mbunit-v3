using System;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides support for performing deferred build actions.
    /// </summary>
    public interface ISupportDeferredActions
    {
        /// <summary>
        /// Registers a deferred action to be performed when <see cref="ApplyDeferredActions" /> is called.
        /// </summary>
        /// <remarks>
        /// Typically used to enable decorations to be applied in a particular order.
        /// </remarks>
        /// <param name="codeElement">The associated code element, used to report errors if the deferred action throws an exception</param>
        /// <param name="order">The order in which the action should be applied, from least order to greatest</param>
        /// <param name="deferredAction">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/>
        /// or <paramref name="deferredAction"/> is null</exception>
        /// <seealso cref="ApplyDeferredActions"/>
        void AddDeferredAction(ICodeElementInfo codeElement, int order, Action deferredAction);

        /// <summary>
        /// Applies all pending deferred in order and clears the list.
        /// </summary>
        void ApplyDeferredActions();
    }
}
