using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Provides information about the contents of an attribute.
    /// </summary>
    public interface IAttributeInfo
    {
        /// <summary>
        /// Gets the attribute type.
        /// </summary>
        ITypeInfo Type { get; }

        /// <summary>
        /// Gets the constructor used to create the attribute.
        /// </summary>
        IConstructorInfo Constructor { get; }

        /// <summary>
        /// Gets the attribute constructor argument values.
        /// </summary>
        object[] ArgumentValues { get; }

        /// <summary>
        /// Gets the attribute field values.
        /// </summary>
        IDictionary<IFieldInfo, object> FieldValues { get; }

        /// <summary>
        /// Gets the attribute property values.
        /// </summary>
        IDictionary<IPropertyInfo, object> PropertyValues { get; }
    }
}
