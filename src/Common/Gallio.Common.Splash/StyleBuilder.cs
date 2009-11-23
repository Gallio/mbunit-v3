using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Builds <see cref="Style" /> objects.
    /// </summary>
    public class StyleBuilder
    {
        /// <summary>
        /// Creates a builder with default style properties.
        /// </summary>
        public StyleBuilder()
        {
        }

        /// <summary>
        /// Creates a builder initialized as a copy of an existing style.
        /// </summary>
        /// <param name="style">The style to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        public StyleBuilder(Style style)
        {
            if (style == null)
                throw new ArgumentNullException("style");
        }

        /// <summary>
        /// Creates an immutable style object from the builder's properties.
        /// </summary>
        /// <returns>The new style object.</returns>
        public Style ToStyle()
        {
            return new Style();
        }
    }
}
