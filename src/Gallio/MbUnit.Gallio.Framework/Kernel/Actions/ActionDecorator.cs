using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Actions
{
    /// <summary>
    /// Represents a method that decorates another action.
    /// </summary>
    /// <typeparam name="T">The type of object the action is performed upon</typeparam>
    /// <param name="obj">The object to act upon</param>
    /// <param name="action">The action to decorate which should be called in
    /// the middle of applying the decoration</param>
    public delegate void ActionDecorator<T>(T obj, Action<T> action);
}
