using System;
using System.Collections.Generic;

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
        /// Gets an attribute field value.
        /// </summary>
        /// <param name="name">The field name</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentException">Thrown if there is no field with the specified name</exception>
        object GetFieldValue(string name);

        /// <summary>
        /// Gets an attribute property value.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentException">Thrown if there is no property with the specified name</exception>
        object GetPropertyValue(string name);

        /// <summary>
        /// Gets the attribute field values.
        /// </summary>
        IDictionary<IFieldInfo, object> FieldValues { get; }

        /// <summary>
        /// Gets the attribute property values.
        /// </summary>
        IDictionary<IPropertyInfo, object> PropertyValues { get; }

        /// <summary>
        /// Gets the attribute as an object.
        /// </summary>
        /// <returns>The attribute</returns>
        object Resolve();
    }
}
