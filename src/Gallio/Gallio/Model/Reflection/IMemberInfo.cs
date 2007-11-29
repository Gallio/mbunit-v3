using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="MemberInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IMemberInfo : ICodeElementInfo
    {
        /// <summary>
        /// Gets the compound name of the member consisting of the compound name of its
        /// declaring type followed by a period and the name of this member.
        /// </summary>
        string CompoundName { get; }

        /// <summary>
        /// Gets the declaring type of the member, or null if this code
        /// element is a type that is not nested inside any other type.
        /// </summary>
        ITypeInfo DeclaringType { get; }

        /// <summary>
        /// Gets the member to its underlying native reflection type.
        /// </summary>
        /// <returns>The underlying native reflection type</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved</exception>
        MemberInfo Resolve();
    }
}
