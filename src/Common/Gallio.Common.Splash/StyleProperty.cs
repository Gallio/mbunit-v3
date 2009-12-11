using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Specifies a style property that may either be set explicitly or inherited from
    /// another style.
    /// </summary>
    /// <typeparam name="T">The type of value held by the property.</typeparam>
    public struct StyleProperty<T> : IEquatable<StyleProperty<T>>
    {
        private readonly bool inherited;
        private readonly T value;

        /// <summary>
        /// Gets a special value that indicates that a style property value is inherited.
        /// </summary>
        public static readonly StyleProperty<T> Inherit = new StyleProperty<T>(true, default(T));

        /// <summary>
        /// Creates a style property with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public StyleProperty(T value)
            : this(false, value)
        {
        }

        private StyleProperty(bool inherited, T value)
        {
            this.inherited = inherited;
            this.value = value;
        }

        /// <summary>
        /// Returns true if the property value is inherited.
        /// </summary>
        public bool Inherited
        {
            get { return inherited; }
        }

        /// <summary>
        /// Gets the value of the property if it is not inherited.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the property is inherited.</exception>
        public T Value
        {
            get
            {
                if (inherited)
                    throw new InvalidOperationException("The property value is inherited.");
                return value;
            }
        }

        /// <summary>
        /// Gets the value of the property if it is not inherited
        /// otherwise returns the inherited value.
        /// </summary>
        /// <param name="inheritedValue">The inherited value.</param>
        /// <returns>The value.</returns>
        public T GetValueOrInherit(T inheritedValue)
        {
            return inherited ? inheritedValue : value;
        }

        /// <summary>
        /// Creates a style property with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The style property.</returns>
        public static implicit operator StyleProperty<T>(T value)
        {
            return new StyleProperty<T>(value);
        }

        /// <summary>
        /// Returns true if the properties are equal.
        /// </summary>
        public static bool operator ==(StyleProperty<T> x, StyleProperty<T> y)
        {
            return x.inherited == y.inherited
                && Equals(x.value, y.value);
        }

        /// <summary>
        /// Returns true if the properties are not equal.
        /// </summary>
        public static bool operator !=(StyleProperty<T> x, StyleProperty<T> y)
        {
            return ! (x == y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is StyleProperty<T>
                && Equals((StyleProperty<T>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return inherited ? 1 : (value != null ? value.GetHashCode() : 0);
        }

        /// <inheritdoc />
        public bool Equals(StyleProperty<T> other)
        {
            return this == other;
        }
    }
}
