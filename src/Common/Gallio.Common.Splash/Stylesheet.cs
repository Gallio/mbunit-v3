using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Manages a collection of styles.
    /// </summary>
    public class Stylesheet
    {
        private readonly Dictionary<string, Style> styles;

        /// <summary>
        /// Creates an empty stylesheet.
        /// </summary>
        public Stylesheet()
        {
            styles = new Dictionary<string, Style>();
        }

        /// <summary>
        /// Adds a style to the stylesheet.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <param name="style">The style to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="style"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already a style with name <paramref name="name"/>.</exception>
        public void AddStyle(string name, Style style)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (style == null)
                throw new ArgumentNullException("style");

            if (styles.ContainsKey(name))
                throw new InvalidOperationException(string.Format("The stylesheet already contains a style called '{0}'.", name));

            styles.Add(name, style);
        }

        /// <summary>
        /// Gets a style by name.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <param name="defaultStyle">The default style to return if no style
        /// is present with the specified name.</param>
        /// <returns>The style.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public Style GetStyle(string name, Style defaultStyle)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Style style;
            if (styles.TryGetValue(name, out style))
                return style;
            return defaultStyle;
        }
    }
}
