using System;
using System.Reflection;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="MethodBase" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IFunctionInfo : IMemberInfo
    {
        /// <summary>
        /// Gets the method modifiers.
        /// </summary>
        MethodAttributes Modifiers { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        /// <returns>The parameters</returns>
        IParameterInfo[] GetParameters();

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <returns>The native reflection target</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved</exception>
        new MethodBase Resolve();
    }
}