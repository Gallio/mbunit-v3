using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="ParameterInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IParameterInfo : ICodeElementInfo, ISlotInfo
    {
        /// <summary>
        /// Gets the member to which the parameter belongs.
        /// </summary>
        IMemberInfo Member { get; }

        /// <summary>
        /// Gets the parameter modifiers.
        /// </summary>
        ParameterAttributes Modifiers { get; }

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <returns>The native reflection target</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved</exception>
        ParameterInfo Resolve();
    }
}
