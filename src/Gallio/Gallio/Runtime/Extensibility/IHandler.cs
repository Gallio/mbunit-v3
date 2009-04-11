using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler activates instances of a plugin, component or traits object.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Activates the instance.
        /// </summary>
        /// <returns>The activated instance</returns>
        object Activate();

        // void Release(object instance);
    }
}
