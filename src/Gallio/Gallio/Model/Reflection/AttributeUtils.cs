using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Manipulates attributes described by their metadata.
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Creates an attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute description</param>
        /// <returns>The attribute instance</returns>
        public static object CreateAttribute(IAttributeInfo attribute)
        {
            ConstructorInfo constructor = attribute.Constructor.Resolve();
            object instance = constructor.Invoke(attribute.ArgumentValues);

            foreach (KeyValuePair<IFieldInfo, object> initializer in attribute.FieldValues)
                initializer.Key.Resolve().SetValue(instance, initializer.Value);

            foreach (KeyValuePair<IPropertyInfo, object> initializer in attribute.PropertyValues)
                initializer.Key.Resolve().SetValue(instance, initializer.Value, null);

            return instance;
        }

        /// <summary>
        /// Returns true if the collection contains attributes of the specified type.
        /// </summary>
        /// <param name="attributes">The attribute descriptions</param>
        /// <param name="attributeType">The attribute type</param>
        /// <returns>True if the enumeration contains at least one attribute of the specified type</returns>
        public static bool HasAttributeOfType(IEnumerable<IAttributeInfo> attributes, Type attributeType)
        {
            string qualifiedTypeName = attributeType.FullName;

            foreach (IAttributeInfo attribute in attributes)
                if (ReflectionUtils.IsDerivedFrom(attribute.Type, qualifiedTypeName))
                    return true;

            return false;
        }

        /// <summary>
        /// Creates instances of all attributes of a specified type from an enumeration.
        /// </summary>
        /// <param name="attributes">The attribute descriptions</param>
        /// <param name="attributeType">The attribute type</param>
        /// <returns>The attribute instances</returns>
        public static IEnumerable<object> CreateAttributesOfType(IEnumerable<IAttributeInfo> attributes, Type attributeType)
        {
            string qualifiedTypeName = attributeType.FullName;

            foreach (IAttributeInfo attribute in attributes)
                if (ReflectionUtils.IsDerivedFrom(attribute.Type, qualifiedTypeName))
                    yield return CreateAttribute(attribute);
        }

        /// <summary>
        /// Gets the attribute of the specified type, or null if none.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="element">The code element</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The attribute, or null if none</returns>
        /// <exception cref="InvalidOperationException">Thrown if the code element
        /// has multiple attributes of the specified type</exception>
        public static T GetAttribute<T>(ICodeElementInfo element, bool inherit) where T : class
        {
            IEnumerator<object> en = element.GetAttributes(typeof(T), inherit).GetEnumerator();
            if (!en.MoveNext())
                return null;

            T attrib = (T) en.Current;

            if (en.MoveNext())
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "There are multiple instances of attribute '{0}'.", typeof(T).FullName));

            return attrib;
        }

        /// <summary>
        /// Gets the attributes of the specified type.
        /// </summary>
        /// <param name="element">The code element</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <returns>The attributes</returns>
        public static IEnumerable<T> GetAttributes<T>(ICodeElementInfo element, bool inherit) where T : class
        {
            foreach (T attrib in element.GetAttributes(typeof(T), inherit))
                yield return attrib;
        }
    }
}
