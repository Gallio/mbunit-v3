using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// A slot represents a field, property or parameter.  It is used to
    /// simplify the handling of data binding since all three of these types
    /// are similar in that they can hold values of some type.
    /// </summary>
    public interface ISlotInfo : ICodeElementInfo
    {
        /// <summary>
        /// Gets the type of value held in the slot.
        /// </summary>
        ITypeInfo ValueType { get; }

        /// <summary>
        /// Gets the positional index of a parameter slot, or 0 in other cases.
        /// </summary>
        int Position { get; }
    }
}
