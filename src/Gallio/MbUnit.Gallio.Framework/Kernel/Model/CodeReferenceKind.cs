using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Describes the kind of code element specified by a <see cref="CodeReference" />.
    /// </summary>
    public enum CodeReferenceKind
    {
        /// <summary>
        /// The code reference is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The code reference specifies an assembly.
        /// </summary>
        Assembly,

        /// <summary>
        /// The code reference specifies a namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// The code reference specifies a type.
        /// </summary>
        Type,

        /// <summary>
        /// The code reference specifies a member.
        /// </summary>
        Member,

        /// <summary>
        /// The code reference specifies a parameter.
        /// </summary>
        Parameter
    }
}
