using System;

namespace MbUnit.Framework.Kernel.Utilities
{
    /// <summary>
    /// A variation on <see cref="EventHandler{T}" /> with a typed sender
    /// parameter to eliminate redundant casts.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The event arguments</param>
    /// <typeparam name="TSender">The sender type</typeparam>
    /// <typeparam name="TEventArgs">The event args type</typeparam>
    public delegate void TypedEventHandler<TSender, TEventArgs>(TSender sender, TEventArgs e)
        where TEventArgs : EventArgs;
}
