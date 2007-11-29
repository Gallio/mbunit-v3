using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="FieldInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IFieldInfo : IMemberInfo, ISlotInfo
    {
        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <returns>The native reflection target</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved</exception>
        new FieldInfo Resolve();
    }
}
